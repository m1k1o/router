using Router.DHCP;
using System;

namespace Router.Controllers.DHCP
{
    class Timers : Controller
    {
        public double LeaseTimeout
        {
            get => DHCPLease.LeaseTimeout.TotalSeconds;
            set => DHCPLease.LeaseTimeout = TimeSpan.FromSeconds(value);
        }

        public double OfferTimeout
        {
            get => DHCPLease.OfferTimeout.TotalSeconds;
            set => DHCPLease.OfferTimeout = TimeSpan.FromSeconds(value);
        }

        public double RenewalTimeout
        {
            get => Protocols.DHCP.RenewalTimeValue.TotalSeconds;
            set => Protocols.DHCP.RenewalTimeValue = TimeSpan.FromSeconds(value);
        }

        public double RebindingTimeout
        {
            get => Protocols.DHCP.RebindingTimeValue.TotalSeconds;
            set => Protocols.DHCP.RebindingTimeValue = TimeSpan.FromSeconds(value);
        }

        public object Export() => this;
    }
}
