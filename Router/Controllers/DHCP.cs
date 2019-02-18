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

        private static JSON DHCPPool(DHCPPool DHCPPool)
        {
            var obj = new JSONObject();
            //obj.Push("id", DHCPPool.ID.ToString());
            obj.Push("first_ip", DHCPPool.FirstIP);
            obj.Push("last_ip", DHCPPool.LastIP);
            obj.Push("is_dynamic", DHCPPool.IsDynamic);
            obj.Push("available", DHCPPool.Available);
            obj.Push("allocated", DHCPPool.Allocated);
            return obj;
        }

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

        public static JSON Pools(string Data = null)
        {
            var obj = new JSONObject();
            foreach (KeyValuePair<Interface, DHCPPool> Entry in Router.DHCP.DHCPPool.Interfaces)
            {
                obj.Push(Entry.Key.ID.ToString(), DHCPPool(Entry.Value));
            }
            return obj;
        }

        public static JSON PoolAdd(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 4)
            {
                return new JSONError("Expected InterfaceID, FisrtIP, LastIP, IsDynamic.");
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
                return new JSONObject(Interface.ID.ToString(), DHCPPool(Pool));
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }
        }

        public static JSON PoolToggle(string Data)
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
                return new JSONObject("is_dynamic", EPool.IsDynamic);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }
        }

        public static JSON PoolRemove(string Data)
        {
            try
            {
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Data);
                Router.DHCP.DHCPPool.Interfaces.Remove(Interface);
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
            obj.Push("pools", Pools());
            return obj;
        }
    }
}
