using Router.Helpers;
using System.Collections.Generic;
using System.Net;

namespace Router.Protocols
{
    class RIPRouteCollection : List<RIPRoute>
    {
        public void Add(IPAddress IPAddress, IPSubnetMask IPSubnetMask, IPAddress NextHop, uint Metric)
        {
            Add(new RIPRoute(IPAddress, IPSubnetMask, NextHop, Metric));
        }

        public override string ToString()
        {
            var result = "RIP Route Collection:\n\n";
            foreach (var item in this)
            {
                result += item.ToString() + "\n\n";
            }

            return result;
        }
    }
}
