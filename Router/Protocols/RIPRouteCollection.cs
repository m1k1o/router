using Router.Helpers;
using System.Collections.Generic;
using System.Net;

namespace Router.Protocols
{
    sealed class RIPRouteCollection : List<RIPRoute>
    {
        public void Add(IPNetwork IPNetwork, IPAddress NextHop, uint Metric)
        {
            Add(new RIPRoute(IPNetwork, NextHop, Metric));
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
