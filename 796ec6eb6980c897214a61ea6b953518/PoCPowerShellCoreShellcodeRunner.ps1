<#
Author: Matthew Graeber (@mattifestation)
License: BSD 3-Clause
#>

function Invoke-VirtualAlloc {
    Param (
        [IntPtr] $lpAddress,
        [UInt32] $dwSize,
        [UInt32] $flAllocationType,
        [UInt32] $flProtect
    )

    $AsmBuilder = [System.Reflection.Assembly].Assembly.GetTypes() | ? {$_.Name -eq 'AssemblyBuilder' }

    $AssemblyBuilder = $AsmBuilder::DefineDynamicAssembly('TestAssembly', 'Run')
    $ModuleBuilder = $AssemblyBuilder.DefineDynamicModule('TestModule', $False)
    $TypeBuilder = $ModuleBuilder.DefineType('Kernel32', [Reflection.TypeAttributes]::Public)
    $MethodBuilder = $TypeBuilder.DefineMethod('VirtualAlloc',
                                               [Reflection.MethodAttributes] 'Public, Static, PinvokeImpl',
                                               [Reflection.CallingConventions] 'Standard',
                                               [IntPtr],
                                               [Type[]] @([IntPtr], [UInt32], [UInt32], [UInt32]))
    $DllImportConstructor = [Runtime.InteropServices.DllImportAttribute].GetConstructor([Type[]] @([String]))

    $Field_EntryPoint = [Runtime.InteropServices.DllImportAttribute].GetField('EntryPoint')
    $Field_CharSet = [Runtime.InteropServices.DllImportAttribute].GetField('CharSet')
    $Field_ExactSpelling = [Runtime.InteropServices.DllImportAttribute].GetField('ExactSpelling')
    $Field_SetLastError = [Runtime.InteropServices.DllImportAttribute].GetField('SetLastError')
    $Field_PreserveSig = [Runtime.InteropServices.DllImportAttribute].GetField('PreserveSig')
    $Field_CallingConvention = [Runtime.InteropServices.DllImportAttribute].GetField('CallingConvention')
    $Field_BestFitMapping = [Runtime.InteropServices.DllImportAttribute].GetField('BestFitMapping')
    $Field_ThrowOnUnmappableChar = [Runtime.InteropServices.DllImportAttribute].GetField('ThrowOnUnmappableChar')

    $FieldInfoArray = @($Field_EntryPoint,
                        $Field_ExactSpelling,
                        $Field_SetLastError,
                        $Field_PreserveSig,
                        $Field_CallingConvention,
                        $Field_BestFitMapping,
                        $Field_ThrowOnUnmappableChar)

    $FieldArguments = @('VirtualAlloc',
                        $False,
                        $True,
                        $True,
                        [Runtime.InteropServices.CallingConvention]::Winapi,
                        $False,
                        $False)

    $CustomAttribBuilder = New-Object Reflection.Emit.CustomAttributeBuilder($DllImportConstructor,
                                                                             'api-ms-win-core-memory-l1-1-0.dll',
                                                                             [Reflection.FieldInfo[]] $FieldInfoArray,
                                                                             [Object[]] $FieldArguments)

    $MethodBuilder.SetCustomAttribute($CustomAttribBuilder)

    $PreserveSigConstructor = [Runtime.InteropServices.PreserveSigAttribute].GetConstructor(@())
    $CustomAttribBuilder = New-Object Reflection.Emit.CustomAttributeBuilder($PreserveSigConstructor, @())
    $MethodBuilder.SetCustomAttribute($CustomAttribBuilder)

    $MethodBuilder.SetImplementationFlags([Reflection.MethodImplAttributes]::PreserveSig)

    $Kernel32 = $TypeBuilder.CreateType()

    $MethodInfo = New-Object Reflection.Emit.DynamicMethod('VirtualAlloc', [IntPtr], @([IntPtr], [UInt32], [UInt32], [UInt32]))
    $Generator = $MethodInfo.GetILGenerator()
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ldarg_0)
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ldarg_1)
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ldarg_2)
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ldarg_3)
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Call, $Kernel32.GetMethod('VirtualAlloc'))
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ret)
    $Delegate = [Func``5[IntPtr, UInt32, UInt32, UInt32, IntPtr]]
    $ProxyMethod = $MethodInfo.CreateDelegate($Delegate)

    $ProxyMethod.Invoke($lpAddress, $dwSize, $flAllocationType, $flProtect)
}

function Invoke-CreateThread {
    Param (
        [IntPtr] $lpThreadAttributes,
        [UInt32] $dwStackSize,
        [IntPtr] $lpStartAddress,
        [IntPtr] $lpParameter,
        [UInt32] $dwCreationFlags,
        [IntPtr] $lpThreadId
    )

    $AsmBuilder = [System.Reflection.Assembly].Assembly.GetTypes() | ? {$_.Name -eq 'AssemblyBuilder' }

    $AssemblyBuilder = $AsmBuilder::DefineDynamicAssembly('TestAssembly_2', 'Run')
    $ModuleBuilder = $AssemblyBuilder.DefineDynamicModule('TestModule', $False)
    $TypeBuilder = $ModuleBuilder.DefineType('Kernel32', [Reflection.TypeAttributes]::Public)
    $MethodBuilder = $TypeBuilder.DefineMethod('CreateThread',
                                               [Reflection.MethodAttributes] 'Public, Static, PinvokeImpl',
                                               [Reflection.CallingConventions] 'Standard',
                                               [IntPtr],
                                               [Type[]] @([IntPtr], [UInt32], [IntPtr], [IntPtr], [UInt32], [IntPtr]))
    $DllImportConstructor = [Runtime.InteropServices.DllImportAttribute].GetConstructor([Type[]] @([String]))

    $Field_EntryPoint = [Runtime.InteropServices.DllImportAttribute].GetField('EntryPoint')
    $Field_CharSet = [Runtime.InteropServices.DllImportAttribute].GetField('CharSet')
    $Field_ExactSpelling = [Runtime.InteropServices.DllImportAttribute].GetField('ExactSpelling')
    $Field_SetLastError = [Runtime.InteropServices.DllImportAttribute].GetField('SetLastError')
    $Field_PreserveSig = [Runtime.InteropServices.DllImportAttribute].GetField('PreserveSig')
    $Field_CallingConvention = [Runtime.InteropServices.DllImportAttribute].GetField('CallingConvention')
    $Field_BestFitMapping = [Runtime.InteropServices.DllImportAttribute].GetField('BestFitMapping')
    $Field_ThrowOnUnmappableChar = [Runtime.InteropServices.DllImportAttribute].GetField('ThrowOnUnmappableChar')

    $FieldInfoArray = @($Field_EntryPoint,
                        $Field_ExactSpelling,
                        $Field_SetLastError,
                        $Field_PreserveSig,
                        $Field_CallingConvention,
                        $Field_BestFitMapping,
                        $Field_ThrowOnUnmappableChar)

    $FieldArguments = @('CreateThread',
                        $False,
                        $True,
                        $True,
                        [Runtime.InteropServices.CallingConvention]::Winapi,
                        $False,
                        $False)

    $CustomAttribBuilder = New-Object Reflection.Emit.CustomAttributeBuilder($DllImportConstructor,
                                                                             'api-ms-win-core-processthreads-l1-1-0.dll',
                                                                             [Reflection.FieldInfo[]] $FieldInfoArray,
                                                                             [Object[]] $FieldArguments)

    $MethodBuilder.SetCustomAttribute($CustomAttribBuilder)

    $PreserveSigConstructor = [Runtime.InteropServices.PreserveSigAttribute].GetConstructor(@())
    $CustomAttribBuilder = New-Object Reflection.Emit.CustomAttributeBuilder($PreserveSigConstructor, @())
    $MethodBuilder.SetCustomAttribute($CustomAttribBuilder)

    $MethodBuilder.SetImplementationFlags([Reflection.MethodImplAttributes]::PreserveSig)

    $Kernel32 = $TypeBuilder.CreateType()

    $MethodInfo = New-Object Reflection.Emit.DynamicMethod('CreateThread', [IntPtr], @([IntPtr], [UInt32], [IntPtr], [IntPtr], [UInt32], [IntPtr]))
    $Generator = $MethodInfo.GetILGenerator()
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ldarg_0)
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ldarg_1)
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ldarg_2)
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ldarg_3)
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ldarg_S, ([Byte] 4))
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ldarg_S, ([Byte] 5))
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Call, $Kernel32.GetMethod('CreateThread'))
    $Generator.Emit([System.Reflection.Emit.OpCodes]::Ret)
    $Delegate = [Func``7[IntPtr, UInt32, IntPtr, IntPtr, UInt32, IntPtr, IntPtr]]
    $ProxyMethod = $MethodInfo.CreateDelegate($Delegate)

    $ProxyMethod.Invoke($lpThreadAttributes,
                        $dwStackSize,
                        $lpStartAddress,
                        $lpParameter,
                        $dwCreationFlags,
                        $lpThreadId)
}


$X64SampleInstructions = [Byte[]] @(
0xB9, 0x03, 0x00, 0x00, 0x00, # mov ecx, 3
0x83, 0xC1, 0x03,             # add ecx, 3
0x31, 0xC0,                   # xor eax, eax
0xC3)                         # ret

<#
# Sample instructions that will crash your PowerShell Core remoting session
$X64SampleInstructions = [Byte[]] @(
0x48, 0x31, 0xC0, # xor rax, rax
0x50,             # push rax
0xC3)             # ret
#>

$ShellcodeAddr = Invoke-VirtualAlloc -lpAddress ([IntPtr]::Zero) -dwSize $X64SampleInstructions.Length -flAllocationType 0x3000 -flProtect 0x40
[System.Runtime.InteropServices.Marshal]::Copy($X64SampleInstructions, 0, $ShellcodeAddr, $X64SampleInstructions.Length)
Invoke-CreateThread -lpThreadAttributes ([IntPtr]::Zero) -dwStackSize 0 -lpStartAddress $ShellcodeAddr -lpParameter ([IntPtr]::Zero) -dwCreationFlags 0 -lpThreadId ([IntPtr]::Zero)
