using System;
using System.Runtime.InteropServices;

namespace WindowTextExtractor.Native
{
    static class Hook
    {
        [DllImport("WindowTextExtractorHook.dll")]
        public static extern bool SetHook(IntPtr hwndCaller, IntPtr hwndTarget, int msg);

        [DllImport("WindowTextExtractorHook.dll")]
        public static extern bool UnsetHook(IntPtr hwndCaller, IntPtr hwndTarget);

        [DllImport("WindowTextExtractorHook.dll")]
        public static extern bool QueryPasswordEdit();

        [DllImport("WindowTextExtractorHook64.dll", EntryPoint = "SetHook")]
        public static extern bool SetHook64(IntPtr hwndCaller, IntPtr hwndTarget, int msg);

        [DllImport("WindowTextExtractorHook64.dll", EntryPoint = "UnsetHook")]
        public static extern bool UnsetHook64(IntPtr hwndCaller, IntPtr hwndTarget);

        [DllImport("WindowTextExtractorHook64.dll", EntryPoint = "QueryPasswordEdit")]
        public static extern bool QueryPasswordEdit64();
    }
}