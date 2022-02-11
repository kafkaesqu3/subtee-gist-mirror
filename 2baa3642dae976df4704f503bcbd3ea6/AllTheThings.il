
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.module extern user32.dll
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Windows.Forms
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly allthethings
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module allthethings.dll
// MVID: {EE106CE7-96CD-449F-8428-EE8FEF315492}
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x05020000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit Program
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  Main() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      "Hello From Main...I Don't Do Anything"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ret
  } // end of method Program::Main

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Program::.ctor

} // end of class Program

.class public auto ansi beforefieldinit Thing0
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  ExecParam(string a) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  2
    .locals init (class [System]System.Diagnostics.Process V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "cmd.exe"
    IL_0006:  call       class [System]System.Diagnostics.Process [System]System.Diagnostics.Process::Start(string)
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  callvirt   instance native int [System]System.Diagnostics.Process::get_MainWindowHandle()
    IL_0012:  ldarg.0
    IL_0013:  call       int32 Thing0::SetWindowText(native int,
                                                     string)
    IL_0018:  pop
    IL_0019:  ret
  } // end of method Thing0::ExecParam

  .method private hidebysig static pinvokeimpl("user32.dll" winapi) 
          int32  SetWindowText(native int hWnd,
                               string text) cil managed preservesig
  {
  }
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Thing0::.ctor

} // end of class Thing0

.class private auto ansi beforefieldinit Exports
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  EntryPoint(native int hwnd,
                                                   native int hinst,
                                                   string lpszCmdLine,
                                                   int32 nCmdShow) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      "EntryPoint"
    IL_0006:  call       void Thing0::ExecParam(string)
    IL_000b:  nop
    IL_000c:  ret
  } // end of method Exports::EntryPoint

  .method public hidebysig static bool  DllRegisterServer() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  1
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "DllRegisterServer"
    IL_0006:  call       void Thing0::ExecParam(string)
    IL_000b:  nop
    IL_000c:  ldc.i4.1
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method Exports::DllRegisterServer

  .method public hidebysig static bool  DllUnregisterServer() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  1
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "DllUnregisterServer"
    IL_0006:  call       void Thing0::ExecParam(string)
    IL_000b:  nop
    IL_000c:  ldc.i4.1
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method Exports::DllUnregisterServer

  .method public hidebysig static void  DllInstall(bool bInstall,
                                                   native int a) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       string [mscorlib]System.Runtime.InteropServices.Marshal::PtrToStringUni(native int)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  call       void Thing0::ExecParam(string)
    IL_000e:  nop
    IL_000f:  ret
  } // end of method Exports::DllInstall

  .method public hidebysig static int32  DllGetClassObject(native int rclsid,
                                                           native int riid,
                                                           native int& ppvObj) cil managed
  {
    // Code size       25 (0x19)
    .export[0]
    .maxstack  1
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       string [mscorlib]System.Runtime.InteropServices.Marshal::PtrToStringUni(native int)
    IL_0007:  stloc.0
    IL_0008:  ldstr      "Hell Yeah! Boomer!"
    IL_000d:  call       valuetype [System.Windows.Forms]System.Windows.Forms.DialogResult [System.Windows.Forms]System.Windows.Forms.MessageBox::Show(string)
    IL_0012:  pop
    IL_0013:  ldc.i4.0
    IL_0014:  stloc.1
    IL_0015:  br.s       IL_0017

    IL_0017:  ldloc.1
    IL_0018:  ret
  } // end of method Exports::DllGetClassObject

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Exports::.ctor

} // end of class Exports


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file AllTheThings.res
