//*************************************************************************************************************************************
//https://github.com/gapotchenko/Gapotchenko.FX/blob/master/Source/Gapotchenko.FX.Diagnostics.Process/Pal/Windows/ProcessEnvironment.cs
//*************************************************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace WindowTextExtractor.Diagnostics
{
    static partial class ProcessEnvironment
    {
        public static string ReadCommandLine(this Process process) => ReadCommandLine(process.Handle);

        public static string ReadCommandLine(IntPtr hProcess)
        {
            int processBitness = ProcessMemory.GetBitness(hProcess);

            if (processBitness == 64)
            {
                // Accessing a 32-bit process.

                var pPeb = _GetPeb64(hProcess);

                if (!ProcessMemory.TryReadIntPtr(hProcess, pPeb + 0x20, out var pProcessParameters))
                    throw new Exception("Unable to read PEB.");

                if (!ProcessMemory.TryReadUInt16(hProcess, pProcessParameters + 0x70, out var commandLineLength))
                    throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                if (!ProcessMemory.TryReadIntPtr(hProcess, pProcessParameters + 0x78, out var pCommandLineBuffer))
                    throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                var adapter = Environment.Is64BitProcess ? (IProcessMemoryAccessor)new ProcessMemoryAccessor(hProcess) : new ProcessMemoryAccessorWow64(hProcess);

                var stream = new ProcessMemoryStream(adapter, pCommandLineBuffer, commandLineLength + sizeof(char));
                var br = new ProcessBinaryReader(stream, Encoding.Unicode);

                return br.ReadCString();
            }
            else
            {
                // Accessing a 32-bit process.

                var pPeb = _GetPeb32(hProcess);

                if (!ProcessMemory.TryReadIntPtr(hProcess, pPeb + 0x10, out var pProcessParameters))
                    throw new Exception("Unable to read PEB.");

                if (!ProcessMemory.TryReadUInt16(hProcess, pProcessParameters + 0x40, out var commandLineLength))
                    throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                if (!ProcessMemory.TryReadIntPtr(hProcess, pProcessParameters + 0x44, out var pCommandLineBuffer))
                    throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                var adapter = new ProcessMemoryAccessor(hProcess);
                var stream = new ProcessMemoryStream(adapter, pCommandLineBuffer, commandLineLength + sizeof(char));
                var br = new ProcessBinaryReader(stream, Encoding.Unicode);

                return br.ReadCString();
            }
        }

        public static bool TryReadEnvironmentVariables(this Process process, out IDictionary<string, string> variables)
        {
            try
            {
                variables = ReadVariables(process.Handle);
                return true;
            }
            catch
            {
                variables = new Dictionary<string, string>();
                return false;
            }
        }

        public static IDictionary<string, string> ReadVariables(IntPtr hProcess)
        {
            int retryCount = 3;
            bool RetryPolicy() => --retryCount > 0;

        Again:
            try
            {
                var stream = _GetEnvStream(hProcess);
                var reader = new ProcessBinaryReader(new BufferedStream(stream), Encoding.Unicode);
                var env = _ReadEnv(reader);

                if (env.Count == 0)
                {
                    // Empty environment may indicate that a process environment block has not been initialized yet.
                    if (RetryPolicy())
                        goto Again;
                }

                return env;
            }
            catch (EndOfStreamException)
            {
                // There may be a race condition in environment block initialization of a recently started process.
                if (RetryPolicy())
                    goto Again;
                else
                    throw;
            }
        }

        static Stream _GetEnvStream(IntPtr hProcess)
        {
            var pEnv = _GetPEnv(hProcess);
            if (pEnv.CanBeRepresentedByNativePointer)
            {
                int dataSize;
                if (!ProcessMemory.HasReadAccess(hProcess, pEnv, out dataSize))
                    throw new Exception("Unable to read process environment block.");

                var provider = new ProcessMemoryAccessor(hProcess);
                return new ProcessMemoryStream(provider, pEnv, dataSize);
            }
            else if (pEnv.Size == 8 && IntPtr.Size == 4)
            {
                // Accessing a 64-bit process from 32-bit host.

                int dataSize;
                try
                {
                    if (!ProcessMemory.HasReadAccessWow64(hProcess, pEnv.ToInt64(), out dataSize))
                        throw new Exception("Unable to read process environment block with WOW64 API.");
                }
                catch (EntryPointNotFoundException)
                {
                    // Windows 10 does not provide NtWow64QueryVirtualMemory64 API call.
                    dataSize = -1;
                }

                var adapter = new ProcessMemoryAccessorWow64(hProcess);
                return new ProcessMemoryStream(adapter, pEnv, dataSize);
            }
            else
            {
                throw new Exception("Unable to access process memory due to unsupported bitness cardinality.");
            }
        }

        static IDictionary<string, string> _ReadEnv(ProcessBinaryReader br)
        {
            var env = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            for (; ; )
            {
                var s = br.ReadCString();
                if (s.Length == 0)
                {
                    // End of environment block.
                    break;
                }

                int j = s.IndexOf('=');
                if (j <= 0)
                    continue;

                string name = s.Substring(0, j);
                string value = s.Substring(j + 1);

                env[name] = value;
            }

            return env;
        }

        static UniPtr _GetPEnv(IntPtr hProcess)
        {
            int processBitness = ProcessMemory.GetBitness(hProcess);

            if (processBitness == 64)
            {
                // Accessing a 64-bit process.

                var pPeb = _GetPeb64(hProcess);

                if (!ProcessMemory.TryReadIntPtr(hProcess, pPeb + 0x20, out var pProcessParameters))
                    throw new Exception("Unable to read PEB.");

                if (!ProcessMemory.TryReadIntPtr(hProcess, pProcessParameters + 0x80, out var pEnv))
                    throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                return pEnv;
            }
            else
            {
                // Accessing a 32-bit process.

                var pPeb = _GetPeb32(hProcess);

                if (!ProcessMemory.TryReadIntPtr(hProcess, pPeb + 0x10, out var pProcessParameters))
                    throw new Exception("Unable to read PEB.");

                if (!ProcessMemory.TryReadIntPtr(hProcess, pProcessParameters + 0x48, out var pEnv))
                    throw new Exception("Unable to read RTL_USER_PROCESS_PARAMETERS.");

                return pEnv;
            }
        }

        /// <summary>
        /// Gets Process Environment Block (PEB) of a 32-bit process.
        /// </summary>
        /// <param name="hProcess">The process handle.</param>
        /// <returns>The PEB base address.</returns>
        static UniPtr _GetPeb32(IntPtr hProcess)
        {
            if (Environment.Is64BitProcess)
            {
                // Getting PEB of a 32-bit process from the 64-bit host.

                var ptr = IntPtr.Zero;
                int pbiSize = IntPtr.Size;
                int res_len = 0;

                int status = NativeMethods.NtQueryInformationProcess(
                    hProcess,
                    NativeMethods.ProcessWow64Information,
                    ref ptr,
                    pbiSize,
                    ref res_len);

                if (status != NativeMethods.STATUS_SUCCESS || res_len != pbiSize)
                    throw new Exception("Unable to query process information.");

                return new UniPtr(ptr.ToInt32());
            }
            else
            {
                // Getting PEB of a 32-bit process from the 32-bit host.

                return _GetPebNative(hProcess);
            }
        }

        /// <summary>
        /// Gets Process Environment Block (PEB) of a 64-bit process.
        /// </summary>
        /// <param name="hProcess">The process handle.</param>
        /// <returns>The PEB base address.</returns>
        static UniPtr _GetPeb64(IntPtr hProcess)
        {
            if (Environment.Is64BitProcess)
            {
                // Getting PEB of a 64-bit process from the 64-bit host.

                return _GetPebNative(hProcess);
            }
            else
            {
                // Getting PEB of a 64-bit process from the 32-bit host using WOW64 API.

                var pbi = new NativeMethods.PROCESS_BASIC_INFORMATION_WOW64();
                int pbiSize = Marshal.SizeOf(pbi);
                int res_len = 0;

                int status = NativeMethods.NtWow64QueryInformationProcess64(
                    hProcess,
                    NativeMethods.ProcessBasicInformation,
                    ref pbi,
                    pbiSize,
                    ref res_len);

                if (status != NativeMethods.STATUS_SUCCESS || res_len != pbiSize)
                    throw new Exception("Unable to query process information.");

                return new UniPtr(pbi.PebBaseAddress);
            }
        }

        static IntPtr _GetPebNative(IntPtr hProcess)
        {
            // Getting PEB of a x-bit process from the y-bit host, where x = y.

            var pbi = new NativeMethods.PROCESS_BASIC_INFORMATION();
            int pbiSize = Marshal.SizeOf(pbi);
            int res_len = 0;

            int status = NativeMethods.NtQueryInformationProcess(
                hProcess,
                NativeMethods.ProcessBasicInformation,
                ref pbi,
                pbiSize,
                ref res_len);

            if (status != NativeMethods.STATUS_SUCCESS || res_len != pbiSize)
                throw new Exception("Unable to query process information.");

            return pbi.PebBaseAddress;
        }
    }
}