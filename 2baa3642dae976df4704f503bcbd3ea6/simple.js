WScript.StdIn.ReadLine();

tyr {
new ActiveXObject('WScript.Shell').Environment('Process')('TMP') = 'C:\\Tools';
// You could add a way to drop this dynamically 	
var manifest = '<?xml version="1.0" encoding="UTF-16" standalone="yes"?> <assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0"> 	<assemblyIdentity type="win32" name="AllTheThings" version="0.0.0.0"/> 	<file name="DynWrapIt.dll">     	<comClass         	description="AllTheThings Class"         	clsid="{89565276-A714-4a43-912E-978BFEEDACDC}"         	threadingModel="Both"         	progid="AllTheThings"/> 	</file>  </assembly>';
var ax = new ActiveXObject("Microsoft.Windows.ActCtx");
ax.ManifestText = manifest;
	
var DWX = ax.CreateObject("AllTheThings");
	
} catch(e) {
		WScript.Echo("Error: " + e);
}
