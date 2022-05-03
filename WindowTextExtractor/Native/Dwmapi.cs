using System;
using System.Runtime.InteropServices;
using WindowTextExtractor.Native.Structs;

namespace WindowTextExtractor.Native
{
    static class Dwmapi
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out Rect pvAttribute, int cbAttribute);
    }
}
