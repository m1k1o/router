using Router.DHCP;
using System.Collections.Generic;

namespace Router.Controllers.DHCP
{
    class Table : Controller
    {
        public static object Entry(DHCPLease DHCPLease) => new
        {
            //id = DHCPLease.ID.ToString(),
            mac = DHCPLease.PhysicalAddress,
            Interface = DHCPLease.Interface.ID.ToString(),
            ip = DHCPLease.IPAddress,

            is_dynamic = DHCPLease.IsDynamic,
            is_offered = DHCPLease.IsOffered,
            is_leased = DHCPLease.IsLeased,
            is_available = DHCPLease.IsAvailable,

            offer_expires_in = DHCPLease.OfferExpiresIn,
            lease_expires_in = DHCPLease.LeaseExpiresIn
        };

        public object Export()
        {
            var DHCPEntries = DHCPTable.Instance.GetEntries();

            var Dictionary = new Dictionary<string, object>();
            foreach (var DHCPEntry in DHCPEntries)
            {
                Dictionary.Add(DHCPEntry.ID.ToString(), Entry(DHCPEntry));
            }

            return Dictionary;
        }
    }
}
