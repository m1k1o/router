using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router
{
    class ARPEntry
    {
        public IPAddress IPAddress;
        public PhysicalAddress PhysicalAddress;
        public DateTime Expires;

        public ARPEntry(IPAddress IPAddress, PhysicalAddress PhysicalAddress, DateTime Expires)
        {
            this.IPAddress = IPAddress;
            this.PhysicalAddress = PhysicalAddress;
            this.Expires = Expires;
        }
    }
}
