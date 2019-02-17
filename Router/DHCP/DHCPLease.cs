using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.DHCP
{
    class DHCPLease
    {
        public static TimeSpan LeaseTimeout { get; set; } = TimeSpan.Parse("01:02:00");
        public static TimeSpan OfferTimeout { get; set; } = TimeSpan.Parse("00:02:00");

        public int ID => GetHashCode();

        public PhysicalAddress PhysicalAddress { get; private set; }
        public Interface Interface { get; private set; }
        public IPAddress IPAddress { get; private set; }
        
        private DateTime OfferedUntil;
        private DateTime LeasedUntil;

        public DHCPLease(PhysicalAddress PhysicalAddress, Interface Interface, IPAddress IPAddress)
        {
            this.PhysicalAddress = PhysicalAddress;
            this.Interface = Interface;
            this.IPAddress = IPAddress;
        }

        public bool IsDynamic { get; set; } = true;

        public bool IsOffered
        {
            get => OfferedUntil == DateTime.MinValue || OfferedUntil >= DateTime.Now;
            set
            {
                if (value && OfferedUntil == DateTime.MinValue)
                {
                    OfferedUntil = DateTime.Now + OfferTimeout;
                }

                if (!value)
                {
                    OfferedUntil = DateTime.MinValue;
                }
            }
        }

        public bool IsLeased
        {
            get => LeasedUntil == DateTime.MinValue || LeasedUntil >= DateTime.Now;
            set
            {
                if (value)
                {
                    LeasedUntil = DateTime.Now + LeaseTimeout;
                }
                else
                {
                    LeasedUntil = DateTime.MinValue;
                }
            }
        }

        public bool ToBeRemoved => !IsLeased && !IsOffered && IsDynamic;

        public int OfferExpiresIn => (int)(OfferedUntil - DateTime.Now).TotalSeconds;

        public int LeaseExpiresIn => (int)(LeasedUntil - DateTime.Now).TotalSeconds;

        public override string ToString()
        {
            return
                "DHCP" + (IsDynamic ? "-Dynamic-" : "-Static-") + "Lease: " + PhysicalAddress.ToString() +" is " + IPAddress.ToString() +
                (IsOffered ? " offered, expires in " + OfferExpiresIn + "sec.") +
                (IsLeased ? " leased for " + LeaseExpiresIn + "sec.");
        }

        // Equality

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = PhysicalAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ Interface.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(DHCPLease DHCPLease)
            => !(DHCPLease is null) && Equals(DHCPLease.PhysicalAddress, PhysicalAddress) && Equals(DHCPLease.Interface, Interface);

        public override bool Equals(object obj)
            => !(obj is null) && obj.GetType() == GetType() && Equals(obj as DHCPLease);
    }
}
