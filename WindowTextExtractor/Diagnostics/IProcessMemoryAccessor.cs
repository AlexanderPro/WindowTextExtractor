//*********************************************************************************************************************************
//https://github.com/gapotchenko/Gapotchenko.FX/blob/master/Source/Gapotchenko.FX.Diagnostics.Process/Pal/IProcessMemoryAccessor.cs
//*********************************************************************************************************************************

namespace WindowTextExtractor.Diagnostics
{
    /// <summary>
    /// Provides low-level access to the process memory.
    /// </summary>
    interface IProcessMemoryAccessor
    {
        /// <summary>
        /// Gets the page size measured in bytes according to the granularity of memory access control.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Reads the process memory.
        /// </summary>
        /// <param name="address">The address to start reading at.</param>
        /// <param name="buffer">The buffer to read to.</param>
        /// <param name="offset">The buffer offset to start reading to.</param>
        /// <param name="count">The count of bytes to read.</param>
        /// <param name="throwOnError">
        /// <para>
        /// Indicates whether to throw an exception on error.
        /// </para>
        /// <para>
        /// The support of this flag is optional; an adapter may just prefer to return -1 even when the flag is <c>true</c>.
        /// </para>
        /// </param>
        /// <returns>The count of read bytes or -1 on error.</returns>
        int ReadMemory(UniPtr address, byte[] buffer, int offset, int count, bool throwOnError);
    }
}
