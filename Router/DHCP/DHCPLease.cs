using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.DHCP
{
    class DHCPLease
    {
        public static TimeSpan LeaseTimeout { get; set; } = TimeSpan.Parse("01:02:00");
        public static TimeSpan OfferTimeout { get; set; } = TimeSpan.Parse("00:02:00");

        public PhysicalAddress PhysicalAddress { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public Interface Interface { get; private set; }

        private DateTime OfferedUntil;
        private DateTime LeasedUntil;

        public DHCPLease(PhysicalAddress PhysicalAddress, IPAddress IPAddress, Interface Interface)
        {
            this.PhysicalAddress = PhysicalAddress;
            this.IPAddress = IPAddress;
            this.Interface = Interface;
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
    }
}
