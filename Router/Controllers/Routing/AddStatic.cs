using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Router.Controllers.Routing
{
    class AddStatic : Controller, Executable
    {
        private RoutingEntry Entry;

        public IPAddress IP { get; set; }
        public IPSubnetMask Mask { get; set; }
        public IPAddress NextHopIp { get; set; }
        public Interface Interface { get; set; }

        public void Execute()
        {
            if (IP == null || Mask == null)
            {
                throw new Exception("Expected IP and Mask.");
            }

            var Network = IPNetwork.Parse(IP, Mask);
            if (NextHopIp == null && Interface == null)
            {
                throw new Exception("You must set NextHopIP and/or InterfaceID.");
            }

            Entry = new RoutingEntry(Network, NextHopIp, Interface, ADistance.Static);
            RoutingTable.Instance.Push(Entry);
        }


        public object Export() => new Dictionary<string, object>()
        {
            { Entry.ID.ToString(), Table.Entry(Entry) }
        };
    }
}
