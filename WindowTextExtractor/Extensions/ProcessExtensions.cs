using System;
using System.Diagnostics;

namespace WindowTextExtractor.Extensions
{
    public static class ProcessExtensions
    {
        public static bool IsWow64Process(this Process process)
        {
            if ((Environment.OSVersion.Version.Major > 5) || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1)))
            {
                bool retVal;
                return NativeMethods.IsWow64Process(process.Handle, out retVal) && retVal;
            }

            return false;
        }
    }
}