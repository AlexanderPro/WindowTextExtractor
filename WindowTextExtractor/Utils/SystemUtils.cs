using System;
using WindowTextExtractor.Native;
using WindowTextExtractor.Native.Enums;

namespace WindowTextExtractor.Utils
{
    static class SystemUtils
    {
        public static void EnableHighDpiSupport()
        {
            if (Environment.OSVersion.Version.Major <= 5)
            {
                return;
            }

            if (Environment.OSVersion.Version >= new Version(6, 3, 0)) // win 8.1 added support for per monitor dpi
            {
                if (Environment.OSVersion.Version >= new Version(10, 0, 15063)) // win 10 creators update added support for per monitor v2
                {
                    User32.SetProcessDpiAwarenessContext(DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                }
                else
                {
                    SHCore.SetProcessDpiAwareness(ProcessDpiAwareness.Process_Per_Monitor_DPI_Aware);
                }
            }
            else
            {
                User32.SetProcessDPIAware();
            }
        }
    }
}
