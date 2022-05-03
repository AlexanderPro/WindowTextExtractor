using System.Drawing;
using System.Runtime.InteropServices;
using WindowTextExtractor.Native.Enums;

namespace WindowTextExtractor.Native.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public ShowWindowCommands showCmd;
        public Point ptMinPosition;
        public Point ptMaxPosition;
        public Rectangle rcNormalPosition;
    }
}
