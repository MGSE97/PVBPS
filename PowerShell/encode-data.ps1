$cert = "$PSScriptRoot\certs\public.cer"

$folder = "$PSScriptRoot\RandomStuff"
$type = "*.png"

if(Test-Path $folder -PathType Container)
{
    # Exists
    Get-ChildItem -Path $folder -Filter $type -Recurse -File | ForEach-Object {
        # Encrypt
        [IO.File]::WriteAllText($_.FullName, (Protect-CmsMessage -To $cert -Content ([Convert]::ToBase64String([IO.File]::ReadAllBytes($_.FullName)))))
    }
}