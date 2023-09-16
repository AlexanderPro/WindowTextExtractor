using System;
using System.Drawing;
using WindowTextExtractor.Native;

namespace WindowTextExtractor.Extensions
{
    public static class GraphicsExtensions
    {
        public static void DrawBorder(this Graphics graphics, IntPtr windowHandle, Pen pen)
        {
            User32.GetWindowRect(windowHandle, out var rect);
            graphics.DrawRectangle(pen, 0, 0, rect.Width, rect.Height);
        }
    }
}
