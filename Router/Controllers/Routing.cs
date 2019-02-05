using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.Controllers
{
    class Routing
    {
        static private readonly RoutingTable RoutingTable = RoutingTable.Instance;

        public JSON Table(string Data = null)
        {
            var arr = new JSONArray();
            var obj = new JSONObject();

            var Rows = RoutingTable.GetEntries();
            foreach (var Row in Rows)
            {
                obj.Empty();

                obj.Push("ip", Row.IPNetwork.NetworkAddress);
                obj.Push("mask", Row.IPNetwork.SubnetMask);
                obj.Push("network", Row.IPNetwork);
                obj.Push("next_hop", Row.NextHopIP);
                obj.Push("interface", Row.Interface);
                obj.Push("type", Row.ADistance);

                arr.Push(obj);
            }

            return arr;
        }
    }
}
