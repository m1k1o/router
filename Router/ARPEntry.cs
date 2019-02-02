using System.Net;
using System.Net.NetworkInformation;

namespace Router
{
    class ARPEntry
    {
        public IPAddress IPAddress;
        public PhysicalAddress PhysicalAddress;
        public long TTL;

        public ARPEntry(IPAddress IPAddress, PhysicalAddress PhysicalAddress, long TTL)
        {
            this.IPAddress = IPAddress;
            this.PhysicalAddress = PhysicalAddress;
            this.TTL = TTL;
        }
    }
}
