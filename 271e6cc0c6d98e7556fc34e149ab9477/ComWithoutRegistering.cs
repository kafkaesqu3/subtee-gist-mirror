using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ComWithoutRegisteringExample
{
	internal static class ComHelper
	{
		private delegate int DllGetClassObject(ref Guid clsid, ref Guid iid, [Out, MarshalAs(UnmanagedType.Interface)] out IClassFactory classFactory);

		internal static object CreateInstance(LibraryModule libraryModule, Guid clsid)
		{
			var classFactory = GetClassFactory(libraryModule, clsid);
			var iid = new Guid("00000000-0000-0000-C000-000000000046"); // IUnknown
			object obj;
			classFactory.CreateInstance(null, ref iid, out obj);
			return obj;
		}

		internal static IClassFactory GetClassFactory(LibraryModule libraryModule, Guid clsid)
		{
			IntPtr ptr = libraryModule.GetProcAddress("DllGetClassObject");
			var callback = (DllGetClassObject) Marshal.GetDelegateForFunctionPointer(ptr, typeof (DllGetClassObject));

			var classFactoryIid = new Guid("00000001-0000-0000-c000-000000000046");
			IClassFactory classFactory;
			var hresult = callback(ref clsid, ref classFactoryIid, out classFactory);

			if (hresult != 0)
			{
				throw new Win32Exception(hresult, "Cannot create class factory");
			}
			return classFactory;
		}
	}


	[Guid("00000001-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IClassFactory
	{
		void CreateInstance([MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);
		void LockServer(bool fLock);
	}

	internal class LibraryModule : IDisposable
	{
		private readonly IntPtr _handle;
		private readonly string _filePath;

		private static class Win32
		{
			[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
			public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

			[DllImport("kernel32.dll")]
			public static extern bool FreeLibrary(IntPtr hModule);

			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern IntPtr LoadLibrary(string lpFileName);
		}


		public static LibraryModule LoadModule(string filePath)
		{
			var libraryModule = new LibraryModule(Win32.LoadLibrary(filePath), filePath);
			if (libraryModule._handle == IntPtr.Zero)
			{
				int error = Marshal.GetLastWin32Error();
				throw new Win32Exception(error, "Cannot load library: " + filePath);
			}

			return libraryModule;
		}

		private LibraryModule(IntPtr handle, string filePath)
		{
			_filePath = filePath;
			_handle = handle;
		}

		~LibraryModule()
		{
			if (_handle != IntPtr.Zero)
			{
				Win32.FreeLibrary(_handle);
			}
		}

		public void Dispose()
		{
			if (_handle != IntPtr.Zero)
			{
				Win32.FreeLibrary(_handle);
			}
			GC.SuppressFinalize(this);
		}

		public IntPtr GetProcAddress(string name)
		{
			IntPtr ptr = Win32.GetProcAddress(_handle, "DllGetClassObject");
			if (ptr == IntPtr.Zero)
			{
				int error = Marshal.GetLastWin32Error();
				string message = string.Format("Cannot find proc {0} in {1}", name, _filePath);
				throw new Win32Exception(error, message);
			}
			return ptr;
		}

		public string FilePath
		{
			get { return _filePath; }
		}
	}
}


