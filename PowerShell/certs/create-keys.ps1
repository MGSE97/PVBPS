if(@(Get-Childitem -Path Cert:\CurrentUser\My -DocumentEncryptionCert -DnsName "pvbps").Length -eq 0)
{
    New-SelfSignedCertificate -DnsName pvbps -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsage KeyEncipherment,DataEncipherment, KeyAgreement -Type DocumentEncryptionCert -NotAfter "1.1.2099" -KeyExportPolicy Exportable;
}
$cert = @(Get-Childitem -Path Cert:\CurrentUser\My -DocumentEncryptionCert -DnsName "pvbps")[0]

$PFXPath = "$PSScriptRoot\cert.pfx"
if(Test-Path $PFXPath)
{
    Remove-Item -Path $PFXPath
}
$PFXPass = ConvertTo-SecureString -String "1234" -Force -AsPlainText
Export-PFXCertificate -Cert $cert -Password $PFXPass -FilePath $PFXPath -NoProperties -NoClobber

$public = "$PSScriptRoot\public.cer"
$private = "$PSScriptRoot\private.cer"

$cert

Export-Certificate -Cert $cert -FilePath $public -NoClobber -Type CERT
Export-Certificate -Cert $cert -FilePath $private -NoClobber -Type CERT
#Set-Content -Path $public -Encoding UTF8 -Value $cert.GetPublicKeyString()
#Set-Content -Path $private -Encoding UTF8 -Value $cert.GetPrivateKeyString()