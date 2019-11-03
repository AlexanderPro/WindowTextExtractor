using System;
using System.Runtime.InteropServices;

namespace WindowTextExtractor
{
    static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(int processID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetConsoleOutputCP();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleOutputCharacter(IntPtr hConsoleOutput, [Out] char[] lpCharacter, uint nLength, Coord dwReadCoord, out uint lpNumberOfCharsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("user32.dll")]
        public static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

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