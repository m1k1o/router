using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router
{
    class ARPEntry
    {
        public static TimeSpan CacheTimeout = TimeSpan.FromSeconds(128);

        public IPAddress IPAddress { get; private set; }

        public PhysicalAddress PhysicalAddress { get; private set; }

        private DateTime Expires;

        public ARPEntry(IPAddress IPAddress, PhysicalAddress PhysicalAddress)
        {
            this.IPAddress = IPAddress;
            this.PhysicalAddress = PhysicalAddress;
            Expires = DateTime.Now + CacheTimeout;
        }

        public bool HasExpired { get => DateTime.Now > Expires; }

        public int ExpiresIn { get => (int)(Expires - DateTime.Now).TotalSeconds; }

        public void Update(PhysicalAddress PhysicalAddress)
        {
            this.PhysicalAddress = PhysicalAddress;
            Expires = DateTime.Now + CacheTimeout;
        }
    }
}
