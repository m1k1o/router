using Router.DHCP;
using Router.Helpers;
using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Controllers
{
    class DHCP
    {
        private static readonly DHCPTable DHCPTable = DHCPTable.Instance;

        private static JSON DHCPLease(DHCPLease DHCPLease)
        {
            var obj = new JSONObject();
            //obj.Push("id", DHCPLease.ID.ToString());
            obj.Push("mac", DHCPLease.PhysicalAddress);
            obj.Push("interface", DHCPLease.Interface.ID.ToString());
            obj.Push("ip", DHCPLease.IPAddress);

            obj.Push("is_dynamic", DHCPLease.IsDynamic);
            obj.Push("is_offered", DHCPLease.IsOffered);
            obj.Push("is_leased", DHCPLease.IsLeased);
            obj.Push("is_available", DHCPLease.IsAvailable);

            obj.Push("offer_expires_in", DHCPLease.OfferExpiresIn);
            obj.Push("lease_expires_in", DHCPLease.LeaseExpiresIn);
            return obj;
        }

        public static JSON Timers(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 4)
                {
                    return new JSONError("Expected LeaseTimeout, OfferTimeout, RenewalTimeValue, RebindingTimeValue.");
                }

                try
                {
                    var LeaseTimeout = TimeSpan.FromSeconds(Int32.Parse(Rows[0]));
                    var OfferTimeout = TimeSpan.FromSeconds(Int32.Parse(Rows[1]));
                    var RenewalTimeValue = TimeSpan.FromSeconds(Int32.Parse(Rows[2]));
                    var RebindingTimeValue = TimeSpan.FromSeconds(Int32.Parse(Rows[3]));

                    Router.DHCP.DHCPLease.LeaseTimeout = LeaseTimeout;
                    Router.DHCP.DHCPLease.OfferTimeout = OfferTimeout;
                    Protocols.DHCP.RenewalTimeValue = RenewalTimeValue;
                    Protocols.DHCP.RebindingTimeValue = RebindingTimeValue;
                }
                catch (Exception e)
                {
                    return new JSONError(e.Message);
                }
            }

            var obj = new JSONObject();
            obj.Push("lease_timeout", Router.DHCP.DHCPLease.LeaseTimeout.TotalSeconds);
            obj.Push("offer_timeout", Router.DHCP.DHCPLease.OfferTimeout.TotalSeconds);
            obj.Push("renewal_timeout", Protocols.DHCP.RenewalTimeValue.TotalSeconds);
            obj.Push("rebinding_timeout", Protocols.DHCP.RebindingTimeValue.TotalSeconds);
            return obj;
        }

        public static JSON AddStatic(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 3)
            {
                return new JSONError("Expected MACAddress, InterfaceID, IPAddress.");
            }

            try
            {
                var MAC = PhysicalAddress.Parse(Rows[0].ToUpper().Replace(":", "-"));
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Rows[1]);
                var IP = IPAddress.Parse(Rows[2]);

                var Entry = DHCPTable.AddStatic(MAC, Interface, IP);
                return new JSONObject(Entry.ID.ToString(), DHCPLease(Entry));
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }
        }

        public static JSON RemoveStatic(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 2)
            {
                return new JSONError("Expected MACAddress, InterfaceID.");
            }

            try
            {
                var MAC = PhysicalAddress.Parse(Rows[0].ToUpper().Replace(":", "-"));
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Rows[1]);

                DHCPTable.RemoveStatic(MAC, Interface);
                return new JSONObject("removed", true);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }
        }

        public static JSON Table(string Data = null)
        {
            var obj = new JSONObject();

            var Rows = DHCPTable.GetEntries();
            foreach (var Row in Rows)
            {
                obj.Push(Row.ID.ToString(), DHCPLease(Row));
            }

            return obj;
        }

        public static JSON Initialize(string Data = null)
        {
            var obj = new JSONObject();
            obj.Push("table", Table());
            obj.Push("timers", Timers());
            return obj;
        }
    }
}
