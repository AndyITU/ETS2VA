using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CruiseControl { 

    public class Speed
    {
        const int PROCESS_WM_READ = 0x0010;

        public static void Main()
        {
            Process process = Process.GetProcessesByName("eurotrucks2")[0];
            Console.WriteLine("ProcessID: " + process.Id);
            Console.ReadKey();

            IntPtr hProcess = OpenProcess(PROCESS_WM_READ, false, process.Id);
            int bytesRead = 0;
            byte[] speed = new byte[8];

            //VA.WriteToLog(process[0].Id);
            ReadProcessMemory((int)hProcess, GetSpeedAddress((int)hProcess, process), speed, speed.Length, ref bytesRead);
            Console.WriteLine(BitConverter.ToSingle(speed, 0));
            Console.WriteLine("bytesRead: " + bytesRead);
            Console.ReadKey();

            //This is where your code goes.  Note that the class and this function (main()) are required for this to work.

            //You also have access to the shared VoiceAttack proxy object VA, which is the same that you will find 
            //used with the VoiceAttack plugin framework.  See the help documentation for more info.

            //VA.ExecuteCommand("myCommand");
            //VA.ParseTokens("The time is {TIME}");
            //VA.GetText("myTextVariable");
            //VA.SetDecimal("myDecimalVariable", 2.2);
            //you get the idea ;)
        }

        public static Int64 GetSpeedAddress(int hProcess, Process proc)
        {
            IntPtr baseOffset = proc.MainModule.BaseAddress;
            int bytesRead = 0;
            byte[] tempPtr = new byte[8];
            ReadProcessMemory(hProcess, (Int64)IntPtr.Add(baseOffset, 0x011DF400), tempPtr, tempPtr.Length, ref bytesRead);
            ReadProcessMemory(hProcess, BitConverter.ToInt64(tempPtr, 0) + 0x0198, tempPtr, tempPtr.Length, ref bytesRead);
            ReadProcessMemory(hProcess, BitConverter.ToInt64(tempPtr, 0) + 0x58, tempPtr, tempPtr.Length, ref bytesRead);
            ReadProcessMemory(hProcess, BitConverter.ToInt64(tempPtr, 0) + 0x08, tempPtr, tempPtr.Length, ref bytesRead);

            byte[] pointerBytes = new byte[8];
            ReadProcessMemory(hProcess, BitConverter.ToInt64(tempPtr, 0) + 0x07E8, pointerBytes, pointerBytes.Length, ref bytesRead);
            long pointer = BitConverter.ToInt64(pointerBytes, 0) + 0x88;
            return pointer;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, Int64 lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    }
}