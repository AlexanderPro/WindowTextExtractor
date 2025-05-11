using System;
using System.Windows.Forms;
using System.Threading;
using WindowTextExtractor.Forms;
using WindowTextExtractor.Utils;
using WindowTextExtractor.Settings;
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
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            Application.ThreadException += OnThreadException;
#if WIN32
            var settings = ApplicationSettingsFile.Read();

            // Enable support of high DPI
            if (settings.HighDpiSupport)
            {
                SystemUtils.EnableHighDpiSupport();
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(settings));
#else
            var hwndCaller = 0;
            var hwndTarget = 0;
            var messageId = 0;
            if (args != null && args.Length > 2 && int.TryParse(args[0], out hwndCaller) && int.TryParse(args[1], out hwndTarget) && int.TryParse(args[2], out messageId))
            {
                var hwndCallerPtr = new IntPtr(hwndCaller);
                var hwndTargetPtr = new IntPtr(hwndTarget);
                Hook.SetHook64(hwndCallerPtr, hwndTargetPtr, messageId);
                Hook.QueryPasswordEdit64();
                Hook.UnsetHook64(hwndCallerPtr, hwndTargetPtr);
            }
            else
            {
                MessageBox.Show("WindowTextExtractor64.exe is not for a manual run.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
#endif
        }

        static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception ?? new Exception("OnCurrentDomainUnhandledException");
            OnThreadException(sender, new ThreadExceptionEventArgs(ex));
        }

        static void OnThreadException(object sender, ThreadExceptionEventArgs e) =>
            MessageBox.Show(e.Exception.Message, AssemblyUtils.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}