$fileIn = "$PSScriptRoot\ransomware.ps1"
$fileOut = "$PSScriptRoot\ransomware-encoded.ps1"

$content = Get-Content -Path $fileIn -Raw
$encoded = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($content))
Set-Content -Path $fileOut -Encoding UTF8 -Value $encoded