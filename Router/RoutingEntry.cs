using Router.Helpers;
using System.Net;

namespace Router
{
    class RoutingEntry
    {
        public int ID => GetHashCode();

        public IPNetwork IPNetwork { get; private set; }
        public IPAddress NextHopIP { get; private set; }
        public Interface Interface { get; private set; }
        public ADistance ADistance { get; private set; }

        public RoutingEntry(IPNetwork IPNetwork, IPAddress NextHopIP, Interface Interface, ADistance ADistance)
        {
            this.IPNetwork = IPNetwork;
            this.NextHopIP = NextHopIP;
            this.Interface = Interface;
            this.ADistance = ADistance;
        }

        public bool HasNextHopIP => NextHopIP != null && NextHopIP is IPAddress;

        public bool HasInterface => Interface != null && Interface is Interface;
        
        public override string ToString()
        {
            string Str = "";
            if (HasNextHopIP)
            {
                Str += NextHopIP.ToString() + " ";
            }

            if (HasInterface)
            {
                Str += "interface " + Interface.ToString() + " ";
            }

            return IPNetwork.ToString() + " via " + Str + "[" + ADistance.ToString() + "]";
        }

        // Equality

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IPNetwork.GetHashCode();
                hashCode = (hashCode * 397) ^ ADistance.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(RoutingEntry RoutingEntry)
            => !(RoutingEntry is null) && Equals(RoutingEntry.IPNetwork, IPNetwork) && RoutingEntry.ADistance == ADistance;

        public override bool Equals(object obj)
            => !(obj is null) && obj.GetType() == GetType() && Equals(obj as RoutingEntry);
    }

    enum ADistance : byte
    {
        DirectlyConnected = 0,
        Static = 1,
        RIP = 120
    }
}
