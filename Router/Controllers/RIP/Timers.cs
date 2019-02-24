using Router.RIP;
using System;

namespace Router.Controllers.RIP
{
    class Timers : Controller
    {
        public double UpdateTimer
        {
            get => RIPUpdates.Timer.TotalSeconds;
            set => RIPUpdates.Timer = TimeSpan.FromSeconds(value);
        }

        public double InvalidTimer
        {
            get => RIPEntryTimers.InvalidTimer.TotalSeconds;
            set => RIPEntryTimers.InvalidTimer = TimeSpan.FromSeconds(value);
        }

        public double HoldTimer
        {
            get => RIPEntryTimers.HoldTimer.TotalSeconds;
            set => RIPEntryTimers.HoldTimer = TimeSpan.FromSeconds(value);
        }

        public double FlushTimer
        {
            get => RIPEntryTimers.FlushTimer.TotalSeconds;
            set => RIPEntryTimers.FlushTimer = TimeSpan.FromSeconds(value);
        }

        public object Export() => this;
    }
}
