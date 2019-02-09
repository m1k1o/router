using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.Controllers
{
    class Routing
    {
        private static readonly RoutingTable RoutingTable = RoutingTable.Instance;

        private JSON RoutingEntry(RoutingEntry RoutingEntry)
        {
            var obj = new JSONObject();
            obj.Push("id", RoutingEntry.ToString());
            obj.Push("ip", RoutingEntry.IPNetwork.NetworkAddress);
            obj.Push("mask", RoutingEntry.IPNetwork.SubnetMask);
            obj.Push("network", RoutingEntry.IPNetwork);
            obj.Push("next_hop", RoutingEntry.NextHopIP);
            obj.Push("interface", RoutingEntry.Interface);
            obj.Push("type", RoutingEntry.ADistance);
            return obj;
        }

        public JSON AddRoute(string Data)
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
                return RoutingEntry(Entry);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }
        }

        public JSON RemoveRoute(string Data)
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

        public JSON Table(string Data = null)
        {
            var obj = new JSONObject();

            var Rows = RoutingTable.GetEntries();
            foreach (var Row in Rows)
            {
                obj.Push(Row.ToString(), RoutingEntry(Row));
            }

            return obj;
        }
    }
}
