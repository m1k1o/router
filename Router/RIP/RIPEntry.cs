using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        public bool AllowUpdates { get; set; } = true;

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
            if (HasChanged && AllowUpdates)
            {
                this.NextHopIP = NextHopIP;
                this.Metric = Metric;
            }

            Update();
            SyncWithRT = true;
            return HasChanged;
        }

        public bool Equals(RIPEntry RIPEntry)
        {
            if (RIPEntry is null)
            {
                return false;
            }

            if (ReferenceEquals(this, RIPEntry))
            {
                return true;
            }

            return RIPEntry.IPNetwork == IPNetwork && RIPEntry.Interface == Interface;
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == GetType() && Equals(obj as RIPEntry);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IPNetwork.GetHashCode();
                hashCode = (hashCode * 397) ^ Interface.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(RIPEntry obj1, RIPEntry obj2)
        {
            if (obj1 is null || obj2 is null)
            {
                return false;
            }

            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(RIPEntry obj1, RIPEntry obj2)
        {
            return !(obj1 == obj2);
        }
    }
}
