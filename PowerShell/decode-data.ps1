$cert = "$PSScriptRoot\certs\cert.pfx"

$folder = "$PSScriptRoot\RandomStuff"
$type = "*.jpg"

if(Test-Path $folder -PathType Container)
{
    # Exists
    Get-ChildItem -Path $folder -Filter $type -Recurse -File | ForEach-Object {
        # Encrypt
        [IO.File]::WriteAllBytes($_.FullName, [Convert]::FromBase64String((UnProtect-CmsMessage -Path $_.FullName)))
    }
}