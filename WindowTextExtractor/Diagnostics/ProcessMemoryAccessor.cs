//****************************************************************************************************************************************
//https://github.com/gapotchenko/Gapotchenko.FX/blob/master/Source/Gapotchenko.FX.Diagnostics.Process/Pal/Windows/ProcessMemoryAccessor.cs
//****************************************************************************************************************************************

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WindowTextExtractor.Diagnostics
{
    sealed class ProcessMemoryAccessor : IProcessMemoryAccessor
    {
        public ProcessMemoryAccessor(IntPtr hProcess)
        {
            m_hProcess = hProcess;
        }

        readonly IntPtr m_hProcess;

        public int PageSize => SystemInfo.PageSize;

        public unsafe int ReadMemory(UniPtr address, byte[] buffer, int offset, int count, bool throwOnError)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset + count > buffer.Length)
                throw new ArgumentException();

            int actualCount = 0;
            bool status;

            fixed (byte* p = buffer)
            {
                status = NativeMethods.ReadProcessMemory(
                    m_hProcess,
                    address,
                    p + offset,
                    count,
                    ref actualCount);
            }

            if (!status)
            {
                if (throwOnError)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != 0)
                        throw new Win32Exception(error);
                }

                return -1;
            }

            return (int)actualCount;
        }
    }
}