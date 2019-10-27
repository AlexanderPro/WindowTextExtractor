using System;
using System.Text;
using System.Runtime.InteropServices;

namespace WindowTextExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    struct Coord
    {
        public short X;
        public short Y;

        public Coord(short x, short y)
        {
            X = x;
            Y = y;
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    struct SmallRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ConsoleScreenBufferInfo
    {
        public Coord dwSize;
        public Coord dwCursorPosition;
        public ushort wAttributes;
        public SmallRect srWindow;
        public Coord dwMaximumWindowSize;
    }
}
