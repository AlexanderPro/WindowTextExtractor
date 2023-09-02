using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WindowTextExtractor.Native;
using WindowTextExtractor.Native.Enums;
using WindowTextExtractor.Native.Structs;

namespace WindowTextExtractor.Extensions
{
    public static class ProcessExtensions
    {
        public static bool IsWow64Process(this Process process)
        {
            if ((Environment.OSVersion.Version.Major > 5) || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1)))
            {
                bool retVal;
                return Kernel32.IsWow64Process(process.Handle, out retVal) && retVal;
            }

            return false;
        }

        public static Process GetParentProcess(this Process process)
        {
            var pbi = new PROCESS_BASIC_INFORMATION();
            int returnLength;
            var status = Ntdll.NtQueryInformationProcess(process.Handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
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

        public static Priority GetProcessPriority(this Process process) => Kernel32.GetPriorityClass(process.Handle) switch
        {
            PriorityClass.REALTIME_PRIORITY_CLASS => Priority.RealTime,
            PriorityClass.HIGH_PRIORITY_CLASS => Priority.High,
            PriorityClass.ABOVE_NORMAL_PRIORITY_CLASS => Priority.AboveNormal,
            PriorityClass.NORMAL_PRIORITY_CLASS => Priority.Normal,
            PriorityClass.BELOW_NORMAL_PRIORITY_CLASS => Priority.BelowNormal,
            PriorityClass.IDLE_PRIORITY_CLASS => Priority.Idle,
            _ => Priority.Normal
        };
    }
}