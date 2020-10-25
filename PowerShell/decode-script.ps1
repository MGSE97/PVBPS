$fileIn = "$PSScriptRoot\ransomware-encoded.ps1"

$content = Get-Content -Path $fileIn -Raw
[Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($content))