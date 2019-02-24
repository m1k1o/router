using Router.Helpers;
using System;
using System.Net;

namespace Router.Controllers
{
    static class Routing
    {
        private static readonly RoutingTable RoutingTable = RoutingTable.Instance;

        private static old_JSON RoutingEntry(RoutingEntry RoutingEntry)
        {
            var obj = new old_JSONObject();
            //obj.Push("id", RoutingEntry.ID);
            obj.Add("ip", RoutingEntry.IPNetwork.NetworkAddress);
            obj.Add("mask", RoutingEntry.IPNetwork.SubnetMask);
            obj.Add("network", RoutingEntry.IPNetwork);
            obj.Add("next_hop", RoutingEntry.HasNextHopIP ? RoutingEntry.NextHopIP.ToString() : null);
            obj.Add("interface", RoutingEntry.HasInterface ? RoutingEntry.Interface.ID.ToString() : null);
            obj.Add("type", RoutingEntry.ADistance);
            return obj;
        }

        public static old_JSON AddStatic(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 4)
            {
                return new old_JSONError("Expected IPAddress, SubnetMask, NextHopIP, InterfaceID.");
            }

            if (string.IsNullOrEmpty(Rows[2]) && string.IsNullOrEmpty(Rows[3]))
            {
                return new old_JSONError("You must set NextHopIP and/or InterfaceID.");
            }

            try
            {
                var Network = IPNetwork.Parse(Rows[0], Rows[1]);
                var NextHopIP = string.IsNullOrEmpty(Rows[2]) ? null : IPAddress.Parse(Rows[2]);
                var Interface = string.IsNullOrEmpty(Rows[3]) ? null : Router.Interfaces.Instance.GetInterfaceById(Rows[3]);

                if (RoutingTable.Find(Network, ADistance.DirectlyConnected) != null)
                {
                    throw new Exception("Network " + Network + " is directly connected.");
                }

                var Entry = new RoutingEntry(Network, NextHopIP, Interface, ADistance.Static);
                RoutingTable.Push(Entry);
                return new old_JSONObject(Entry.ID.ToString(), RoutingEntry(Entry));
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
                return new old_JSONError("Expected IPAddress, SubnetMask.");
            }

            IPNetwork IPNetwork;
            try
            {
                IPNetwork = IPNetwork.Parse(Rows[0], Rows[1]);
            }
            catch (Exception e)
            {
                return new old_JSONError(e.Message);
            }

            return new old_JSONObject("removed", RoutingTable.Remove(IPNetwork, ADistance.Static));
        }

        public static old_JSON Lookup(string Data)
        {
            var Rows = Data.Split('\n');

            // Validate
            if (Rows.Length != 1)
            {
                return new old_JSONError("Expected IPAddress.");
            }
            
            IPAddress IPAddress;
            try
            {
                IPAddress = IPAddress.Parse(Rows[0]);
            }
            catch (Exception e)
            {
                return new old_JSONError(e.Message);
            }

            RoutingEntry BestMatch = Router.RoutingTable.Instance.Lookup(IPAddress);

            // Answer
            return BestMatch != null ? RoutingEntry(BestMatch) : (new old_JSONObject("found", false));
        }


        public static old_JSON Table(string Data = null)
        {
            var obj = new old_JSONObject();

            var Rows = RoutingTable.BestEntries();
            foreach (var Row in Rows)
            {
                obj.Add(Row.ID.ToString(), RoutingEntry(Row));
            }

            return obj;
        }

        public static old_JSON Initialize(string Data = null)
        {
            var obj = new old_JSONObject();
            obj.Add("table", Table());
            return obj;
        }
    }
}
