using Router.Helpers;
using System.Net;

namespace Router
{
    class RoutingEntry
    {
        public IPNetwork IPNetwork;
        public IPAddress NextHopIP;
        public Interface Interface;
        public ADistance ADistance;

        public RoutingEntry(IPNetwork IPNetwork, IPAddress NextHopIP, Interface Interface, ADistance ADistance)
        {
            this.IPNetwork = IPNetwork;
            this.NextHopIP = NextHopIP;
            this.Interface = Interface;
            this.ADistance = ADistance;
        }

        public bool HasNextHopIP()
        {
            return NextHopIP != null && NextHopIP is IPAddress;
        }

        public bool HasInterface()
        {
            return Interface != null && Interface is Interface;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IPNetwork.GetHashCode();
                hashCode = (hashCode * 397) ^ ADistance.GetHashCode();
                return hashCode;
            }
        }
        public override string ToString()
        {
            return ADistance.ToString() + " " + IPNetwork.ToString();
        }
    }

    public enum ADistance : byte
    {
        DirectlyConnected = 0,
        Static = 1,
        RIP = 120
    }
}
