using System;
using System.Windows.Forms;
using WindowTextExtractor.Forms;
using WindowTextExtractor.Native;

namespace WindowTextExtractor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if WIN32
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
#else
            var hwndCaller = 0;
            var hwndTarget = 0;
            var messageId = 0;
            if (args != null && args.Length > 2 && int.TryParse(args[0], out hwndCaller) && int.TryParse(args[1], out hwndTarget) && int.TryParse(args[2], out messageId))
            {
                var hwndCallerPtr = new IntPtr(hwndCaller);
                var hwndTargetPtr = new IntPtr(hwndTarget);
                NativeMethods.SetHook64(hwndCallerPtr, hwndTargetPtr, messageId);
                NativeMethods.QueryPasswordEdit64();
                NativeMethods.UnsetHook64(hwndCallerPtr, hwndTargetPtr);
            }
#endif
        }
    }
}