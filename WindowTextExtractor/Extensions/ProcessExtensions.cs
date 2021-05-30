using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WindowTextExtractor.Native;

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

        public static Process GetParentProcess(this Process process)
        {
            var pbi = new PROCESS_BASIC_INFORMATION();
            int returnLength;
            var status = NativeMethods.NtQueryInformationProcess(process.Handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
            if (status != 0)
            {
                return null;
            }

            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch
            {
                return null;
            }
        }

        public static Priority GetProcessPriority(this Process process)
        {
            var priorityClass = NativeMethods.GetPriorityClass(process.Handle);
            switch (priorityClass)
            {
                case PriorityClass.REALTIME_PRIORITY_CLASS: return Priority.RealTime;
                case PriorityClass.HIGH_PRIORITY_CLASS: return Priority.High;
                case PriorityClass.ABOVE_NORMAL_PRIORITY_CLASS: return Priority.AboveNormal;
                case PriorityClass.NORMAL_PRIORITY_CLASS: return Priority.Normal;
                case PriorityClass.BELOW_NORMAL_PRIORITY_CLASS: return Priority.BelowNormal;
                case PriorityClass.IDLE_PRIORITY_CLASS: return Priority.Idle;
                default: return Priority.Normal;
            }
        }
    }
}