using Router.Helpers;
using Router.RIP;
using System;

namespace Router.Controllers
{
    static class RIP
    {
        private static readonly RIPTable RIPTable = RIPTable.Instance;

        private static JSON RIPEntry(RIPEntry RIPEntry)
        {
            var obj = new JSONObject();
            //obj.Push("id", RIPEntry.ID.ToString());
            obj.Push("ip", RIPEntry.IPNetwork.NetworkAddress);
            obj.Push("mask", RIPEntry.IPNetwork.SubnetMask);
            obj.Push("network", RIPEntry.IPNetwork);
            obj.Push("next_hop", RIPEntry.NextHopIP);
            obj.Push("interface", RIPEntry.Interface.ID.ToString());
            obj.Push("metric", RIPEntry.Metric);

            obj.Push("never_updated", RIPEntry.NeverUpdated);
            obj.Push("possibly_down", RIPEntry.PossibblyDown);
            obj.Push("in_hold", RIPEntry.InHold);

            obj.Push("sync_with_rt", RIPEntry.SyncWithRT);
            obj.Push("can_be_updated", RIPEntry.CanBeUpdated);
            obj.Push("timers_enabled", RIPEntry.TimersEnabled);
            obj.Push("since_last_update", RIPEntry.SinceLastUpdate);
            return obj;
        }

        public static JSON Timers(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 4)
                {
                    return new JSONError("Expected UpdateTimer, InvalidTimer, HoldTimer, FlushTimer.");
                }

                try
                {
                    var UpdateTimer = TimeSpan.FromSeconds(Int32.Parse(Rows[0]));
                    var InvalidTimer = TimeSpan.FromSeconds(Int32.Parse(Rows[1]));
                    var HoldTimer = TimeSpan.FromSeconds(Int32.Parse(Rows[2]));
                    var FlushTimer = TimeSpan.FromSeconds(Int32.Parse(Rows[3]));

                    RIPUpdates.Timer = UpdateTimer;
                    RIPEntryTimers.InvalidTimer = InvalidTimer;
                    RIPEntryTimers.HoldTimer = HoldTimer;
                    RIPEntryTimers.FlushTimer = FlushTimer;
                }
                catch (Exception e)
                {
                    return new JSONError(e.Message);
                }
            }

            var obj = new JSONObject();
            obj.Push("update_timer", RIPUpdates.Timer.TotalSeconds);
            obj.Push("invalid_timer", RIPEntryTimers.InvalidTimer.TotalSeconds);
            obj.Push("hold_timer", RIPEntryTimers.HoldTimer.TotalSeconds);
            obj.Push("flush_timer", RIPEntryTimers.FlushTimer.TotalSeconds);
            return obj;
        }
        
        public static JSON Table(string Data = null)
        {
            var obj = new JSONObject();

            var Rows = RIPTable.GetEntries();
            foreach (var Row in Rows)
            {
                obj.Push(Row.ID.ToString(), RIPEntry(Row));
            }

            return obj;
        }

        public static JSON Initialize(string Data = null)
        {
            var obj = new JSONObject();
            obj.Push("table", Table());
            obj.Push("timers", Timers());
            return obj;
        }
    }
}
