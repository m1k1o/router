using Router.RIP;
using System.Collections.Generic;

namespace Router.Controllers.RIP
{
    class Table : Controller
    {
        public static object Entry(RIPEntry RIPEntry) => new
        {
            //id = RIPEntry.ID.ToString(),
            ip = RIPEntry.IPNetwork.NetworkAddress,
            mask = RIPEntry.IPNetwork.SubnetMask,
            network = RIPEntry.IPNetwork.ToString(),
            next_hop = RIPEntry.NextHopIP,
            Interface = RIPEntry.Interface.ID.ToString(),
            metric = RIPEntry.Metric,

            never_updated = RIPEntry.NeverUpdated,
            possibly_down = RIPEntry.PossibblyDown,
            in_hold = RIPEntry.InHold,

            sync_with_rt = RIPEntry.SyncWithRT,
            can_be_updated = RIPEntry.CanBeUpdated,
            timers_enabled = RIPEntry.TimersEnabled,
            since_last_update = RIPEntry.SinceLastUpdate,
        };

        public object Export()
        {
            // TODO: Bad Practices.
            RIPTable.Instance.SyncWithRT();

            var RIPEntries = RIPTable.Instance.GetEntries();

            var Dictionary = new Dictionary<string, object>();
            foreach (var RIPEntry in RIPEntries)
            {
                Dictionary.Add(RIPEntry.ID.ToString(), Entry(RIPEntry));
            }

            return Dictionary;
        }
    }
}
