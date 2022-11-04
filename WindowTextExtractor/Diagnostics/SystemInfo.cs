//*****************************************************************************************************************************
//https://github.com/gapotchenko/Gapotchenko.FX/blob/master/Source/Gapotchenko.FX.Diagnostics.Process/Pal/Windows/SystemInfo.cs
//*****************************************************************************************************************************

namespace WindowTextExtractor.Diagnostics
{
    static class SystemInfo
    {
        static SystemInfo()
        {
            NativeMethods.GetSystemInfo(out var systemInfo);
            PageSize = checked((int)systemInfo.dwPageSize);
        }

        public static int PageSize { get; }

        public static class Native
        {
            static Native()
            {
                NativeMethods.GetNativeSystemInfo(out var systemInfo);
                PageSize = checked((int)systemInfo.dwPageSize);
            }

            public static int PageSize { get; }
        }
    }
}