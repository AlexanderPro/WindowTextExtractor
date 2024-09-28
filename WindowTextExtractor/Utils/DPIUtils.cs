using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowTextExtractor.Native;
using WindowTextExtractor.Native.Enums;
using WindowTextExtractor.Native.Structs;

namespace WindowTextExtractor.Utils
{
    public static class DPIUtils
    {
        /// <summary>
        /// Min OS version build that supports DPI per monitor
        /// </summary>
        private const int MinOSVersionBuild = 14393;

        /// <summary>
        /// Min OS version major build that support DPI per monitor
        /// </summary>
        private const int MinOSVersionMajor = 10;

        /// <summary>
        /// Flag, if OS supports DPI per monitor
        /// </summary>
        private static bool _isSupportingDpiPerMonitor;

        /// <summary>
        /// Flag, if OS version checked
        /// </summary>
        private static bool _isOSVersionChecked;

        /// <summary>
        /// Flag, if OS supports DPI per monitor
        /// </summary>
        internal static bool IsSupportingDpiPerMonitor
        {
            get
            {
                if (_isOSVersionChecked)
                {
                    return _isSupportingDpiPerMonitor;
                }

                _isOSVersionChecked = true;
                var osVersionInfo = new OSVERSIONINFOEXW
                {
                    dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEXW))
                };

                if (Ntdll.RtlGetVersion(ref osVersionInfo) != 0)
                {
                    _isSupportingDpiPerMonitor = Environment.OSVersion.Version.Major >= MinOSVersionMajor && Environment.OSVersion.Version.Build >= MinOSVersionBuild;

                    return _isSupportingDpiPerMonitor;
                }

                _isSupportingDpiPerMonitor = osVersionInfo.dwMajorVersion >= MinOSVersionMajor && osVersionInfo.dwBuildNumber >= MinOSVersionBuild;

                return _isSupportingDpiPerMonitor;
            }
        }

        /// <summary>
        /// Get scale factor for an each monitor
        /// </summary>
        /// <param name="control"> Any control for OS who doesn't support DPI per monitor </param>
        /// <param name="monitorPoint"> Monitor point (Screen.Bounds) </param>
        /// <returns> Scale factor </returns>
        public static decimal ScaleFactor(Control control, Point monitorPoint)
        {
            var dpi = GetDpi(control, monitorPoint);

            return dpi * 100 / 96.0m;
        }

        /// <summary>
        /// Get DPI for a monitor
        /// </summary>
        /// <param name="control"> Any control for OS who doesn't support DPI per monitor </param>
        /// <param name="monitorPoint"> Monitor point (Screen.Bounds) </param>
        /// <returns> DPI </returns>
        public static uint GetDpi(Control control, Point monitorPoint)
        {
            uint dpiX;

            if (IsSupportingDpiPerMonitor)
            {
                var monitorFromPoint = User32.MonitorFromPoint(monitorPoint, 2);

                Shcore.GetDpiForMonitor(monitorFromPoint, DpiType.Effective, out dpiX, out _);
            }
            else
            {
                // If using with System.Windows.Forms - can be used Control.DeviceDpi
                // TODO: For .NET Framework 4.7 and later uncomment
                // dpiX = control == null ? 96 : (uint)control.DeviceDpi;
                dpiX = 96;
            }

            return dpiX;
        }
    }
}
