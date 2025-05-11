using System;
using System.Runtime.InteropServices;
using WindowTextExtractor.Native.Enums;

namespace WindowTextExtractor.Native
{
    static class SHCore
    {
        [DllImport("Shcore.dll")]
        public static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

        [DllImport("SHCore.dll", SetLastError = true)]
        public static extern bool SetProcessDpiAwareness(ProcessDpiAwareness processDpiAwareness);
    }
}
