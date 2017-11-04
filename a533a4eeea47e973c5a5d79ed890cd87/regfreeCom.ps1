# Make Sure dynwrapx,dll is in %temp%
$a = new-object -com Microsoft.Windows.ActCtx
$a.ManifestURL = 'https://gist.githubusercontent.com/subTee/36df32293bc5006148bb6b03b5c4b2c1/raw/661b5aafd55288930761d9ad4eabe7403146ab5c/dynwrapx.dll.manifest'

$b = $a.CreateObject("DynamicWrapperX")
$b.Register("user32.dll", "MessageBoxW", "i=hwwu", "r=l") | Out-Null
$b.MessageBoxW(0, "Hello, world!", "Test", 4) | Out-Null

