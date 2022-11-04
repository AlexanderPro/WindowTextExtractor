//*********************************************************************************************************************************************
//https://github.com/gapotchenko/Gapotchenko.FX/blob/master/Source/Gapotchenko.FX.Diagnostics.Process/Pal/Windows/ProcessMemoryAccessorWow64.cs
//*********************************************************************************************************************************************

using System;

namespace WindowTextExtractor.Diagnostics
{
    sealed class ProcessMemoryAccessorWow64 : IProcessMemoryAccessor
    {
        public ProcessMemoryAccessorWow64(IntPtr hProcess)
        {
            m_hProcess = hProcess;
        }

        readonly IntPtr m_hProcess;

        public int PageSize => SystemInfo.Native.PageSize;

        public unsafe int ReadMemory(UniPtr address, byte[] buffer, int offset, int count, bool throwOnError)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset + count > buffer.Length)
                throw new ArgumentException();

            long actualCount = 0;
            int status;

            fixed (byte* p = buffer)
            {
                status = NativeMethods.NtWow64ReadVirtualMemory64(
                    m_hProcess,
                    address.ToInt64(),
                    p + offset,
                    count,
                    ref actualCount);
            }

            if (status != NativeMethods.STATUS_SUCCESS)
                return -1;

            return (int)actualCount;
        }
    }
}