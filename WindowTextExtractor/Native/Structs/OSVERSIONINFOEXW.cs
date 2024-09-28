using System.Runtime.InteropServices;

namespace WindowTextExtractor.Native.Structs
{
    /// <summary>
    /// Contains operating system version information.
    /// </summary>
    /// <remarks>
    /// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-osversioninfoexw"/>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct OSVERSIONINFOEXW
    {
        /// <summary>
        /// The size of this data structure, in bytes
        /// </summary>
        public int dwOSVersionInfoSize;

        /// <summary>
        /// The major version number of the operating system.
        /// </summary>
        public int dwMajorVersion;

        /// <summary>
        /// The minor version number of the operating system.
        /// </summary>
        public int dwMinorVersion;

        /// <summary>
        /// The build number of the operating system.
        /// </summary>
        public int dwBuildNumber;

        /// <summary>
        /// The operating system platform.
        /// </summary>
        public int dwPlatformId;

        /// <summary>
        /// A null-terminated string, such as "Service Pack 3", that indicates the latest Service Pack installed on the system.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;

        /// <summary>
        /// The major version number of the latest Service Pack installed on the system. 
        /// </summary>
        public ushort wServicePackMajor;

        /// <summary>
        /// The minor version number of the latest Service Pack installed on the system.
        /// </summary>
        public ushort wServicePackMinor;

        /// <summary>
        /// A bit mask that identifies the product suites available on the system. 
        /// </summary>
        public short wSuiteMask;

        /// <summary>
        /// Any additional information about the system.
        /// </summary>
        public byte wProductType;

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        public byte wReserved;
    }
}
