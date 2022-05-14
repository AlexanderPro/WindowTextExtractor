using System;
using System.Threading;
using WindowTextExtractor.Native;

namespace WindowTextExtractor
{
    class AccurateTimer
    {
        private int _timerId;
        private Action _action;
        private Winmm.TimerEventDelegate _handler;

        public AccurateTimer(Action action, int delay)
        {
            _action = action;
            Winmm.timeBeginPeriod(1);
            _handler = new Winmm.TimerEventDelegate(TimerCallback);
            _timerId = Winmm.timeSetEvent(delay, 0, _handler, IntPtr.Zero, Constants.EVENT_TYPE);
        }

        public void Stop()
        {
            Winmm.timeKillEvent(_timerId);
            Winmm.timeEndPeriod(1);
            Thread.Sleep(100);
        }

        private void TimerCallback(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (_timerId != 0)
            {
                _action();
            }
        }
    }
}
