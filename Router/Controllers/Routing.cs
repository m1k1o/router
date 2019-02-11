using Router.Helpers;
using System;
using System.Net;

namespace Router.Controllers
{
    static class Routing
    {
        private static readonly RoutingTable RoutingTable = RoutingTable.Instance;

        private static JSON RoutingEntry(RoutingEntry RoutingEntry)
        {
            var obj = new JSONObject();
            //obj.Push("id", RoutingEntry.ID);
            obj.Push("ip", RoutingEntry.IPNetwork.NetworkAddress);
            obj.Push("mask", RoutingEntry.IPNetwork.SubnetMask);
            obj.Push("network", RoutingEntry.IPNetwork);
            obj.Push("next_hop", RoutingEntry.HasNextHopIP ? RoutingEntry.NextHopIP.ToString() : null);
            obj.Push("interface", RoutingEntry.HasInterface ? RoutingEntry.Interface.ID.ToString() : null);
            obj.Push("type", RoutingEntry.ADistance);
            return obj;
        }

        public static JSON AddStatic(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 4)
            {
                return new JSONError("Expected IPAddress, SubnetMask, NextHopIP, InterfaceID.");
            }

            if (string.IsNullOrEmpty(Rows[2]) && string.IsNullOrEmpty(Rows[3]))
            {
                return new JSONError("You must set NextHopIP and/or InterfaceID.");
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
                return new JSONObject(Entry.ID.ToString(), RoutingEntry(Entry));
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
                return new JSONError("Expected IPAddress, SubnetMask.");
            }

            IPNetwork IPNetwork;
            try
            {
                IPNetwork = IPNetwork.Parse(Rows[0], Rows[1]);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            return new JSONObject("removed", RoutingTable.Remove(IPNetwork, ADistance.Static));
        }

        public static JSON Lookup(string Data)
        {
            var Rows = Data.Split('\n');

            // Validate
            if (Rows.Length != 1)
            {
                return new JSONError("Expected IPAddress.");
            }
            
            IPAddress IPAddress;
            try
            {
                IPAddress = IPAddress.Parse(Rows[0]);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            RoutingEntry BestMatch = Router.RoutingTable.Instance.Lookup(IPAddress);

            // Answer
            return BestMatch != null ? RoutingEntry(BestMatch) : (new JSONObject("found", false));
        }


        public static JSON Table(string Data = null)
        {
            var obj = new JSONObject();

            var Rows = RoutingTable.BestEntries();
            foreach (var Row in Rows)
            {
                obj.Push(Row.ID.ToString(), RoutingEntry(Row));
            }

            return obj;
        }

        public static JSON Initialize(string Data = null)
        {
            var obj = new JSONObject();
            obj.Push("table", Table());
            return obj;
        }
    }
}
