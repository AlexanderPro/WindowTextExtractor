using System;
using System.Runtime.InteropServices;

namespace WindowTextExtractor.Native
{
    static class Winmm
    {
        public delegate void TimerEventDelegate(int id, int msg, IntPtr user, int dw1, int dw2);

        [DllImport("winmm.dll")]
        public static extern int timeBeginPeriod(int msec);

        [DllImport("winmm.dll")]
        public static extern int timeEndPeriod(int msec);

        [DllImport("winmm.dll")]
        public static extern int timeSetEvent(int delay, int resolution, TimerEventDelegate handler, IntPtr user, int eventType);

        [DllImport("winmm.dll")]
        public static extern int timeKillEvent(int id);
    }
}
