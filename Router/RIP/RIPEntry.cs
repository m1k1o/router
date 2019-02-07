using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.RIP
{
    class RIPEntry : RIPTimers
    {
        public Interface Interface;
        public IPNetwork IPNetwork;
        public IPAddress NextHopIP;
        public uint Metric;

        public RIPEntry(Interface Interface, IPNetwork IPNetwork, IPAddress NextHopIP, uint Metric)
        {
            this.Interface = Interface;
            this.IPNetwork = IPNetwork;
            this.NextHopIP = NextHopIP;
            this.Metric = Metric;
            Create();
        }

        public bool Update(IPAddress NextHopIP, uint Metric)
        {
            var HasChanged = this.NextHopIP != NextHopIP || this.Metric != Metric;
            if (HasChanged)
            {
                this.NextHopIP = NextHopIP;
                this.Metric = Metric;
            }

            Update();
            return HasChanged;
        }
    }
}
