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
        static private readonly RoutingTable RoutingTable = RoutingTable.Instance;

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

            IPNetwork IPNetwork;
            IPAddress NextHopIP;
            Interface Interface;
            try
            {
                IPNetwork = IPNetwork.Parse(Rows[0], Rows[1]);
                NextHopIP = string.IsNullOrEmpty(Rows[2]) ? null : IPAddress.Parse(Rows[2]);
                Interface = string.IsNullOrEmpty(Rows[3]) ? null : Router.Interfaces.Instance.GetInterfaceById(Rows[3]);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            if (RoutingTable.Find(IPNetwork, ADistance.DirectlyConnected) != null)
            {
                return new JSONError("Netrwork " + IPNetwork + " is directly connected.");
            }

            if (RoutingTable.Find(IPNetwork, ADistance.Static) != null)
            {
                return new JSONError("Netrwork S" + IPNetwork + " is already in table.");
            }

            var Entry = new RoutingEntry(IPNetwork, NextHopIP, Interface, ADistance.Static);
            RoutingTable.Push(Entry);
            return RoutingEntry(Entry);
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
