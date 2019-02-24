using Router.DHCP;
using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Controllers
{
    class DHCP
    {
        private static readonly DHCPTable DHCPTable = DHCPTable.Instance;

        private static old_JSON DHCPPool(DHCPPool DHCPPool)
        {
            var obj = new old_JSONObject();
            //obj.Push("id", DHCPPool.ID.ToString());
            obj.Add("first_ip", DHCPPool.FirstIP);
            obj.Add("last_ip", DHCPPool.LastIP);
            obj.Add("is_dynamic", DHCPPool.IsDynamic);
            obj.Add("available", DHCPPool.Available);
            obj.Add("allocated", DHCPPool.Allocated);
            return obj;
        }

        private static old_JSON DHCPLease(DHCPLease DHCPLease)
        {
            var obj = new old_JSONObject();
            //obj.Push("id", DHCPLease.ID.ToString());
            obj.Add("mac", DHCPLease.PhysicalAddress);
            obj.Add("interface", DHCPLease.Interface.ID.ToString());
            obj.Add("ip", DHCPLease.IPAddress);

            obj.Add("is_dynamic", DHCPLease.IsDynamic);
            obj.Add("is_offered", DHCPLease.IsOffered);
            obj.Add("is_leased", DHCPLease.IsLeased);
            obj.Add("is_available", DHCPLease.IsAvailable);

            obj.Add("offer_expires_in", DHCPLease.OfferExpiresIn);
            obj.Add("lease_expires_in", DHCPLease.LeaseExpiresIn);
            return obj;
        }

        public static old_JSON Timers(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 4)
                {
                    return new old_JSONError("Expected LeaseTimeout, OfferTimeout, RenewalTimeValue, RebindingTimeValue.");
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
                    return new old_JSONError(e.Message);
                }
            }

            var obj = new old_JSONObject();
            obj.Add("lease_timeout", Router.DHCP.DHCPLease.LeaseTimeout.TotalSeconds);
            obj.Add("offer_timeout", Router.DHCP.DHCPLease.OfferTimeout.TotalSeconds);
            obj.Add("renewal_timeout", Protocols.DHCP.RenewalTimeValue.TotalSeconds);
            obj.Add("rebinding_timeout", Protocols.DHCP.RebindingTimeValue.TotalSeconds);
            return obj;
        }

        public static old_JSON AddStatic(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 3)
            {
                return new old_JSONError("Expected MACAddress, InterfaceID, IPAddress.");
            }

            try
            {
                var MAC = Utilities.ParseMAC(Rows[0]);
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Rows[1]);
                var IP = IPAddress.Parse(Rows[2]);

                var Entry = DHCPTable.AddStatic(MAC, Interface, IP);
                return new old_JSONObject(Entry.ID.ToString(), DHCPLease(Entry));
            }
            catch (Exception e)
            {
                return new old_JSONError(e.Message);
            }
        }

        public static old_JSON RemoveStatic(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 2)
            {
                return new old_JSONError("Expected MACAddress, InterfaceID.");
            }

            try
            {
                var MAC = Utilities.ParseMAC(Rows[0]);
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Rows[1]);

                DHCPTable.RemoveStatic(MAC, Interface);
                return new old_JSONObject("removed", true);
            }
            catch (Exception e)
            {
                return new old_JSONError(e.Message);
            }
        }

        public static old_JSON Pools(string Data = null)
        {
            var obj = new old_JSONObject();
            foreach (KeyValuePair<Interface, DHCPPool> Entry in Router.DHCP.DHCPPool.Interfaces)
            {
                obj.Add(Entry.Key.ID.ToString(), DHCPPool(Entry.Value));
            }
            return obj;
        }

        public static old_JSON PoolAdd(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 4)
            {
                return new old_JSONError("Expected InterfaceID, FisrtIP, LastIP, IsDynamic.");
            }

            try
            {
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Rows[0]);
                var FirstIP = IPAddress.Parse(Rows[1]);
                var LastIP = IPAddress.Parse(Rows[2]);
                var IsDynamic = Rows[3] == "true";

                // New Pool
                var Pool = new DHCPPool(FirstIP, LastIP)
                {
                    IsDynamic = IsDynamic
                };

                // Add new Pool
                Router.DHCP.DHCPPool.Interfaces.Add(Interface, Pool);
                return new old_JSONObject(Interface.ID.ToString(), DHCPPool(Pool));
            }
            catch (Exception e)
            {
                return new old_JSONError(e.Message);
            }
        }

        public static old_JSON PoolToggle(string Data)
        {
            try
            {
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Data);

                if (!Router.DHCP.DHCPPool.Interfaces.ContainsKey(Interface))
                {
                    throw new Exception("Pool not available.");
                }

                var EPool = Router.DHCP.DHCPPool.Interfaces[Interface];
                EPool.IsDynamic = !EPool.IsDynamic;
                return new old_JSONObject("is_dynamic", EPool.IsDynamic);
            }
            catch (Exception e)
            {
                return new old_JSONError(e.Message);
            }
        }

        public static old_JSON PoolRemove(string Data)
        {
            try
            {
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Data);
                Router.DHCP.DHCPPool.Interfaces.Remove(Interface);
                return new old_JSONObject("removed", true);
            }
            catch (Exception e)
            {
                return new old_JSONError(e.Message);
            }
        }

        public static old_JSON Table(string Data = null)
        {
            var obj = new old_JSONObject();

            var Rows = DHCPTable.GetEntries();
            foreach (var Row in Rows)
            {
                obj.Add(Row.ID.ToString(), DHCPLease(Row));
            }

            return obj;
        }

        public static old_JSON Initialize(string Data = null)
        {
            var obj = new old_JSONObject();
            obj.Add("table", Table());
            obj.Add("timers", Timers());
            obj.Add("pools", Pools());
            return obj;
        }
    }
}
