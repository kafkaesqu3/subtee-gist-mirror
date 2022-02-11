var DX = new ActiveXObject("DynamicWrapperCS");
var objArray  = new Array(0, "hi", "hey", 0);
DX.Register("user32.dll", "System.Int32", "MessageBoxA", "i=lssl", objArray );
