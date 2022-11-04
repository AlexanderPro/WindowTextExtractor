//********************************************************************************************************************************
//https://github.com/gapotchenko/Gapotchenko.FX/blob/master/Source/Gapotchenko.FX.Diagnostics.Process/Pal/Windows/ProcessMemory.cs
//********************************************************************************************************************************

using System;
using System.Runtime.InteropServices;

namespace WindowTextExtractor.Diagnostics
{
    static class ProcessMemory
    {
        public static int GetBitness(IntPtr hProcess)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                if (!NativeMethods.IsWow64Process(hProcess, out var wow64))
                    return 32;
                if (wow64)
                    return 32;
                return 64;
            }
            else
            {
                return 32;
            }
        }

        static unsafe bool ReadProcessMemory(IntPtr hProcess, UniPtr from, void* to, int length, out int actualLength)
        {
            if (from.CanBeRepresentedByNativePointer)
            {
                actualLength = 0;
                return NativeMethods.ReadProcessMemory(hProcess, from, to, length, ref actualLength);
            }
            else if (from.Size == 8 && IntPtr.Size == 4)
            {
                long numberOfBytesRead = 0;
                var status = NativeMethods.NtWow64ReadVirtualMemory64(hProcess, from.ToInt64(), to, length, ref numberOfBytesRead);
                actualLength = (int)numberOfBytesRead;
                return status == NativeMethods.STATUS_SUCCESS;
            }
            else
            {
                throw new Exception("Unable to access process memory due to unsupported bitness cardinality.");
            }
        }

        public static unsafe bool TryReadIntPtr(IntPtr hProcess, UniPtr address, out UniPtr value)
        {
            if (address.Size == 8)
            {
                long data;
                const int length = sizeof(long);

                bool status = ReadProcessMemory(hProcess, address, &data, length, out var actualLength);

                value = new UniPtr(data);
                return status && actualLength == length;
            }
            else if (address.Size == 4)
            {
                int data;
                const int length = sizeof(int);

                bool status = ReadProcessMemory(hProcess, address, &data, length, out var actualLength);

                value = new UniPtr(data);
                return status && actualLength == length;
            }
            else
            {
                throw new NotSupportedException("Unsupported UniPtr size.");
            }
        }

        public static unsafe bool TryReadUInt16(IntPtr hProcess, UniPtr address, out ushort value)
        {
            ushort data;
            const int length = sizeof(ushort);

            bool status = ReadProcessMemory(hProcess, address, &data, length, out var actualLength);

            value = data;
            return status && actualLength == length;
        }

        public static bool HasReadAccess(IntPtr hProcess, IntPtr address, out int size)
        {
            size = 0;

            var memInfo = new NativeMethods.MEMORY_BASIC_INFORMATION();
            int result = NativeMethods.VirtualQueryEx(
                hProcess,
                address,
                ref memInfo,
                Marshal.SizeOf(memInfo));

            if (result == 0)
                return false;

            if (memInfo.Protect == NativeMethods.PAGE_NOACCESS || memInfo.Protect == NativeMethods.PAGE_EXECUTE)
                return false;

            try
            {
                size = Convert.ToInt32(memInfo.RegionSize.ToInt64() - (address.ToInt64() - memInfo.BaseAddress.ToInt64()));
            }
            catch (OverflowException)
            {
                return false;
            }

            if (size <= 0)
                return false;

            return true;
        }

        public static bool HasReadAccessWow64(IntPtr hProcess, long address, out int size)
        {
            size = 0;

            NativeMethods.MEMORY_BASIC_INFORMATION_WOW64 memInfo;

            int memInfoLength = Marshal.SizeOf(typeof(NativeMethods.MEMORY_BASIC_INFORMATION_WOW64));

            const int memInfoAlign = 8;

            long resultLength = 0;
            int result;

            IntPtr hMemInfo = Marshal.AllocHGlobal(memInfoLength + memInfoAlign * 2);
            try
            {
                // Align to 64 bits.
                var hMemInfoAligned = new IntPtr(hMemInfo.ToInt64() & ~(memInfoAlign - 1L));

                result = NativeMethods.NtWow64QueryVirtualMemory64(
                    hProcess,
                    address,
                    NativeMethods.MEMORY_INFORMATION_CLASS.MemoryBasicInformation,
                    hMemInfoAligned,
                    memInfoLength,
                    ref resultLength);

                memInfo = (NativeMethods.MEMORY_BASIC_INFORMATION_WOW64)Marshal.PtrToStructure(hMemInfoAligned, typeof(NativeMethods.MEMORY_BASIC_INFORMATION_WOW64));
            }
            finally
            {
                Marshal.FreeHGlobal(hMemInfo);
            }

            if (result != NativeMethods.STATUS_SUCCESS)
                return false;

            if (memInfo.Protect == NativeMethods.PAGE_NOACCESS || memInfo.Protect == NativeMethods.PAGE_EXECUTE)
                return false;

            try
            {
                size = Convert.ToInt32(memInfo.RegionSize - (address - memInfo.BaseAddress));
            }
            catch (OverflowException)
            {
                return false;
            }

            if (size <= 0)
                return false;

            return true;
        }
    }
}