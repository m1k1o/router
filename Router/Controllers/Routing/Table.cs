using System.Collections.Generic;

namespace Router.Controllers.Routing
{
    class Table : Controller
    {
        public static object Entry(RoutingEntry RoutingEntry) => new
        {
            ip = RoutingEntry.IPNetwork.NetworkAddress,
            mask = RoutingEntry.IPNetwork.SubnetMask,
            network = RoutingEntry.IPNetwork.ToString(),
            next_hop = RoutingEntry.HasNextHopIP ? RoutingEntry.NextHopIP.ToString() : null,
            Interface = RoutingEntry.HasInterface ? RoutingEntry.Interface.ID.ToString() : null,
            type = RoutingEntry.ADistance.ToString(),
        };

        public object Export()
        {
            var RoutingEntries = RoutingTable.Instance.BestEntries();

            var Dictionary = new Dictionary<string, object>();
            foreach (var RoutingEntry in RoutingEntries)
            {
                Dictionary.Add(RoutingEntry.ID.ToString(), Entry(RoutingEntry));
            }

            return Dictionary;
        }
    }
}
