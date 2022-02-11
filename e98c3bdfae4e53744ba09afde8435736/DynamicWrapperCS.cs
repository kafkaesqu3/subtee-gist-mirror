using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Text;
using System.Runtime.InteropServices;
using System.EnterpriseServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
 
 
// Invoke a Win32 P/Invoke call.
// Based on work by Lee Holmes
// http://www.leeholmes.com/blog/2006/07/21/get-the-owner-of-a-process-in-powershell-pinvoke-and-refout-parameters

/*
Author: Casey Smith, Twitter: @subTee
License: BSD 3-Clause
Create Your Strong Name Key -> key.snk
$key = 'BwIAAAAkAABSU0EyAAQAAAEAAQBhXtvkSeH85E31z64cAX+X2PWGc6DHP9VaoD13CljtYau9SesUzKVLJdHphY5ppg5clHIGaL7nZbp6qukLH0lLEq/vW979GWzVAgSZaGVCFpuk6p1y69cSr3STlzljJrY76JIjeS4+RhbdWHp99y8QhwRllOC0qu/WxZaffHS2te/PKzIiTuFfcP46qxQoLR8s3QZhAJBnn9TGJkbix8MTgEt7hD1DC2hXv7dKaC531ZWqGXB54OnuvFbD5P2t+vyvZuHNmAy3pX0BDXqwEfoZZ+hiIk1YUDSNOE79zwnpVP1+BN0PK5QCPCS+6zujfRlQpJ+nfHLLicweJ9uT7OG3g/P+JpXGN0/+Hitolufo7Ucjh+WvZAU//dzrGny5stQtTmLxdhZbOsNDJpsqnzwEUfL5+o8OhujBHDm/ZQ0361mVsSVWrmgDPKHGGRx+7FbdgpBEq3m15/4zzg343V9NBwt1+qZU+TSVPU0wRvkWiZRerjmDdehJIboWsx4V8aiWx8FPPngEmNz89tBAQ8zbIrJFfmtYnj1fFmkNu3lglOefcacyYEHPX/tqcBuBIg/cpcDHps/6SGCCciX3tufnEeDMAQjmLku8X4zHcgJx6FpVK7qeEuvyV0OGKvNor9b/WKQHIHjkzG+z6nWHMoMYV5VMTZ0jLM5aZQ6ypwmFZaNmtL6KDzKv8L1YN2TkKjXEoWulXNliBpelsSJyuICplrCTPGGSxPGihT3rpZ9tbLZUefrFnLNiHfVjNi53Yg4='
$Content = [System.Convert]::FromBase64String($key)
Set-Content key.snk -Value $Content -Encoding Byte
C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe /r:System.EnterpriseServices.dll /target:library /out:DynamicWrapperCS.dll /keyfile:key.snk DynamicWrapperCS.cs
C:\Windows\Microsoft.NET\Framework\v2.0.50727\regsvcs.exe DynamicWrapperCS.dll


// Requires Admin Rights to Register 
// C:\Windows\Microsoft.NET\Framework\v2.0.50727\regsvcs.exe DynamicWrapperCS.dll
*/
//https://www.add-in-express.com/creating-addins-blog/2011/12/20/type-name-system-comobject/ 

[ComVisible(true)]
[Guid("00000000-ACDC-FACE-9D8E-C0FFEEA5ACDC")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]	
[ProgId("DynamicWrapperCS")]    
public class DynamicWrapperCS : ServicedComponent
{
	
	public DynamicWrapperCS() {} //Basic Constructor
	
	[ComVisible(true)]
	public Object Register(string dllName, string strReturnType,
	string methodName, string strInputParameterTypes,ref object objParameters)
	{
		
		//COM has no Type class, so do the necessary conversions
		Type returnType = Type.GetType(strReturnType);
		//Input Parameter Types
		int countOfInputParameters = (strInputParameterTypes.Length - 2);
		Type[] parameterTypes = new Type[countOfInputParameters];
		for(int i = 2, j = 0; i < strInputParameterTypes.Length; i++, j++)
		{
			parameterTypes[j] = ConvertStringNameToType(strInputParameterTypes[i]);
		}
		
		
		
		// Begin to build the dynamic assembly
		AppDomain domain = AppDomain.CurrentDomain;
		AssemblyName name = new System.Reflection.AssemblyName("PInvokeAssembly");
		AssemblyBuilder assembly = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
		ModuleBuilder module = assembly.DefineDynamicModule("PInvokeModule");
		TypeBuilder type = module.DefineType("PInvokeType", TypeAttributes.Public | TypeAttributes.BeforeFieldInit);
		 
		// Define the actual P/Invoke method
		MethodBuilder method = type.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.PinvokeImpl, returnType, parameterTypes);
		 
		// Apply the P/Invoke constructor
		ConstructorInfo ctor = typeof (DllImportAttribute).GetConstructor (new Type [] { typeof (string) });
		CustomAttributeBuilder attr = new System.Reflection.Emit.CustomAttributeBuilder(ctor, new Object[] { dllName });
		method.SetCustomAttribute(attr);
		
		
		Object[] parameters = ConvertJsArray(objParameters);
		// Create the temporary type, and invoke the method.
		Type realType = type.CreateType();
		return realType.InvokeMember(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, parameters);
		
	}
	
	private Type ConvertStringNameToType(char letter)
	{
		
		switch(letter)
		{
			case 'l':
                    return Type.GetType("System.Int32");
			case 's': 
					return Type.GetType("System.String");
			default:
					return Type.GetType("System.Object");
				
		}
	}
	
	private object[] ConvertJsArray(object jsArray)
      {
         int arrayLength =  (int) jsArray.GetType().InvokeMember("length", BindingFlags.GetProperty, null, jsArray , new object[] { });
         object[] array = new object[arrayLength];

         for (int index = 0; index < arrayLength; index++)
         {
            array[index] = jsArray.GetType().InvokeMember(index.ToString(), BindingFlags.GetProperty, null, jsArray, new object[] { });
         }

         return array;
      }
	
	
	[ComVisible(true)]
	public Object InvokeWin32(string dllName, Type returnType,
	string methodName, Type[] parameterTypes, Object[] parameters)
	{
		  
		  
		// Begin to build the dynamic assembly
		AppDomain domain = AppDomain.CurrentDomain;
		AssemblyName name = new System.Reflection.AssemblyName("PInvokeAssembly");
		AssemblyBuilder assembly = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
		ModuleBuilder module = assembly.DefineDynamicModule("PInvokeModule");
		TypeBuilder type = module.DefineType("PInvokeType", TypeAttributes.Public | TypeAttributes.BeforeFieldInit);
		 
		// Define the actual P/Invoke method
		MethodBuilder method = type.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.PinvokeImpl, returnType, parameterTypes);
		 
		// Apply the P/Invoke constructor
		ConstructorInfo ctor = typeof (DllImportAttribute).GetConstructor (new Type [] { typeof (string) });
		CustomAttributeBuilder attr = new System.Reflection.Emit.CustomAttributeBuilder(ctor, new Object[] { dllName });
		method.SetCustomAttribute(attr);
		 
		// Create the temporary type, and invoke the method.
		Type realType = type.CreateType();
		return realType.InvokeMember(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, parameters);
	}
	
	[ComVisible(true)]
	public Object MessageBox(Int32 hWnd, string lpText, string lpCaption, Int32 uType) 
	{ 
	   Type[]  parameterTypes = { Type.GetType("System.Int32"), Type.GetType("System.String"),Type.GetType("System.String"),Type.GetType("System.Int32")};
	   Object[] parameters = {hWnd, lpText, lpCaption, uType};
	 
	   return InvokeWin32("user32.dll", Type.GetType("System.Int32"), "MessageBoxA", parameterTypes,  parameters );
	}
	
	
	
	
}
