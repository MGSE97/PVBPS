using System;
using System.IO;
using System.Windows.Forms;

namespace KeyLogger
{
    public static class Logger
    {
        public static string RunFile { get; set; }

        /// <summary>
        /// Log exceptions to file
        /// </summary>
        public static void LogException<T>(T exception)
        {

            var logFile = Path.Combine(Path.GetDirectoryName(RunFile) ?? string.Empty, "run");

            using var log = File.AppendText(logFile);
            log.WriteLine($"{DateTime.UtcNow:O}: {exception}");

            File.SetAttributes(logFile, File.GetAttributes(logFile) | FileAttributes.Hidden | FileAttributes.Compressed);
        }
    }
}