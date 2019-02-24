using Router.ARP;
using System;

namespace Router.Controllers.ARP
{
    class Timers : Controller
    {
        public ushort CacheTimeout
        {
            get => (ushort)ARPEntry.CacheTimeout.TotalSeconds;
            set => ARPEntry.CacheTimeout = TimeSpan.FromSeconds(value);
        }

        public double RequestTimeout
        {
            get => ARPMiddleware.RequestTimeout.TotalMilliseconds;
            set => ARPMiddleware.RequestTimeout = TimeSpan.FromMilliseconds(value);
        }

        public double RequestInterval
        {
            get => ARPMiddleware.RequestInterval.TotalMilliseconds;
            set => ARPMiddleware.RequestInterval = TimeSpan.FromMilliseconds(value);
        }

        public object Export() => this;
    }
}
