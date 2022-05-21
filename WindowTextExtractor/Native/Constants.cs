namespace WindowTextExtractor.Native
{
    static class Constants
    {
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_COPYDATA = 0x004A;
        public const int STD_OUTPUT_HANDLE = -11;
        public const int SMTO_ABORTIFHUNG = 0x2;

        public const int WM_GETTEXT = 0x000D;
        public const int WM_GETTEXTLENGTH = 0x000E;
        public const int WM_GETFONT = 0x0031;

        // WindowLong
        public const int GWL_WNDPROC = -4;
        public const int GWL_HINSTANCE = -6;
        public const int GWL_HWNDPARENT = -8;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int GWL_USERDATA = -21;
        public const int GWL_ID = -12;

        // ClassLong
        public const int GCL_STYLE = -26;
        public const int GCL_WNDPROC = -24;
        public const int DWL_DLGPROC = 4;
        public const int DWL_USER = 8;

        public const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

        public const int EVENT_TYPE = 1;

        public const int CURSOR_SHOWING = 0x00000001;

        public const int DI_COMPAT = 0x0004;

        public const int DI_NORMAL = 0x0003;
    }
}
