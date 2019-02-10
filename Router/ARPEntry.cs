using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router
{
    class ARPEntry
    {
        public static TimeSpan CacheTimeout { get; set; } = TimeSpan.FromSeconds(128);

        public int ID => GetHashCode();

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

        public override string ToString()
        {
            return "ARPEntry:" + IPAddress.ToString() + " has " + PhysicalAddress.ToString() + " (timeout: " + ExpiresIn + ")\n";
        }

        // Equality

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IPAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ PhysicalAddress.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(ARPEntry ARPEntry)
            => !(ARPEntry is null) && Equals(ARPEntry.IPAddress, IPAddress) && Equals(ARPEntry.PhysicalAddress, PhysicalAddress);

        public override bool Equals(object obj)
            => !(obj is null) && obj.GetType() == GetType() && Equals(obj as ARPEntry);
    }
}
