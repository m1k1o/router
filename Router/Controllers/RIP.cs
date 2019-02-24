using Router.Helpers;
using Router.RIP;
using System;

namespace Router.Controllers
{
    static class RIP
    {
        private static readonly RIPTable RIPTable = RIPTable.Instance;

        private static old_JSON RIPEntry(RIPEntry RIPEntry)
        {
            var obj = new old_JSONObject();
            //obj.Push("id", RIPEntry.ID.ToString());
            obj.Add("ip", RIPEntry.IPNetwork.NetworkAddress);
            obj.Add("mask", RIPEntry.IPNetwork.SubnetMask);
            obj.Add("network", RIPEntry.IPNetwork);
            obj.Add("next_hop", RIPEntry.NextHopIP);
            obj.Add("interface", RIPEntry.Interface.ID.ToString());
            obj.Add("metric", RIPEntry.Metric);

            obj.Add("never_updated", RIPEntry.NeverUpdated);
            obj.Add("possibly_down", RIPEntry.PossibblyDown);
            obj.Add("in_hold", RIPEntry.InHold);

            obj.Add("sync_with_rt", RIPEntry.SyncWithRT);
            obj.Add("can_be_updated", RIPEntry.CanBeUpdated);
            obj.Add("timers_enabled", RIPEntry.TimersEnabled);
            obj.Add("since_last_update", RIPEntry.SinceLastUpdate);
            return obj;
        }

        public static old_JSON Timers(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 4)
                {
                    return new old_JSONError("Expected UpdateTimer, InvalidTimer, HoldTimer, FlushTimer.");
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
                    return new old_JSONError(e.Message);
                }
            }

            var obj = new old_JSONObject();
            obj.Add("update_timer", RIPUpdates.Timer.TotalSeconds);
            obj.Add("invalid_timer", RIPEntryTimers.InvalidTimer.TotalSeconds);
            obj.Add("hold_timer", RIPEntryTimers.HoldTimer.TotalSeconds);
            obj.Add("flush_timer", RIPEntryTimers.FlushTimer.TotalSeconds);
            return obj;
        }
        
        public static old_JSON Table(string Data = null)
        {
            // TODO: Bad Practices.
            RIPTable.Instance.SyncWithRT();

            var obj = new old_JSONObject();

            var Rows = RIPTable.GetEntries();
            foreach (var Row in Rows)
            {
                obj.Add(Row.ID.ToString(), RIPEntry(Row));
            }

            return obj;
        }

        public static old_JSON Initialize(string Data = null)
        {
            var obj = new old_JSONObject();
            obj.Add("table", Table());
            obj.Add("timers", Timers());
            return obj;
        }
    }
}
