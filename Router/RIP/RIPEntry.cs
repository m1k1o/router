using Router.Helpers;
using System;
using System.Net;

namespace Router.RIP
{
    class RIPEntry : RIPEntryTimers
    {
        public Interface Interface { get; private set; }
        public IPNetwork IPNetwork { get; private set; }
        public IPAddress NextHopIP { get; private set; }

        private uint metric;
        public uint Metric {
            get => metric;
            private set {
                if (value < 0 || value > 15)
                {
                    throw new Exception("Invalid Metric, use number from interval <1;15>");
                }

                metric = value;
            }
        }

        public bool SyncWithRT { get; set; } = true;
        public bool CanBeUpdated { get; set; } = true;

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
            if (HasChanged && CanBeUpdated)
            {
                this.NextHopIP = NextHopIP;
                this.Metric = Metric;
            }

            Update();
            SyncWithRT = true;
            return HasChanged;
        }
        
        public override string ToString()
        {
            return IPNetwork.ToString() + " via " + Interface.ToString();
        }

        // Equality

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IPNetwork.GetHashCode();
                hashCode = (hashCode * 397) ^ Interface.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(RIPEntry RIPEntry)
            => !(RIPEntry is null) && Equals(RIPEntry.IPNetwork, IPNetwork) && Equals(RIPEntry.Interface, Interface);

        public override bool Equals(object obj) =>
            !(obj is null) && obj.GetType() == GetType() && Equals(obj as RIPEntry);
    }
}
