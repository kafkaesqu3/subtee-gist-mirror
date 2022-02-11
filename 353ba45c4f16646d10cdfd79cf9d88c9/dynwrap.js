var actCtx = new ActiveXObject( "Microsoft.Windows.ActCtx" );
actCtx.Manifest = "C:\\Tools\\COM\\dynwrap.test.manifest";
try
{
	var DX = actCtx.CreateObject("DynamicWrapperX");
	DX.Register("user32.dll", "MessageBoxW", "i=hwwu", "r=l");  // Register a dll function.
	res = DX.MessageBoxW(0, "Hello, world!", "Test", 4);        // Call the function.
}
catch(e){ WScript.Echo("Fail");}