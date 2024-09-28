using System;
using System.Runtime.InteropServices;
using WindowTextExtractor.Native.Structs;

namespace WindowTextExtractor.Native
{
    static class Ntdll
    {
        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION pbi, int processInformationLength, out int returnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int RtlGetVersion(ref OSVERSIONINFOEXW versionInfo);
    }
}
