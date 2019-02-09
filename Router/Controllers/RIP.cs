using Router.Helpers;
using Router.RIP;
using System;
using System.Collections.Generic;

namespace Router.Controllers
{
    class RIP
    {
        static private readonly RIPTable RIPTable = RIPTable.Instance;

        private JSON RIPEntry(RIPEntry RIPEntry)
        {
            var obj = new JSONObject();
            obj.Push("id", RIPEntry.ToString());
            obj.Push("ip", RIPEntry.IPNetwork.NetworkAddress);
            obj.Push("mask", RIPEntry.IPNetwork.SubnetMask);
            obj.Push("network", RIPEntry.IPNetwork);
            obj.Push("next_hop", RIPEntry.NextHopIP);
            obj.Push("interface", RIPEntry.Interface);
            obj.Push("metric", RIPEntry.Metric);

            obj.Push("never_updated", RIPEntry.NeverUpdated);
            obj.Push("possibly_down", RIPEntry.PossibblyDown);
            obj.Push("in_hold", RIPEntry.InHold);

            obj.Push("sync_with_rt", RIPEntry.SyncWithRT);
            obj.Push("allow_updates", RIPEntry.AllowUpdates);
            return obj;
        }

        public JSON Timers(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 4)
                {
                    return new JSONError("Expected UpdateTimer, InvalidTimer, HoldTimer, FlushTimer.");
                }

                TimeSpan UpdateTimer;
                TimeSpan InvalidTimer;
                TimeSpan HoldTimer;
                TimeSpan FlushTimer;
                try
                {
                    UpdateTimer = TimeSpan.FromSeconds(Int32.Parse(Rows[0]));
                    InvalidTimer = TimeSpan.FromSeconds(Int32.Parse(Rows[1]));
                    HoldTimer = TimeSpan.FromSeconds(Int32.Parse(Rows[2]));
                    FlushTimer = TimeSpan.FromSeconds(Int32.Parse(Rows[3]));
                }
                catch (Exception e)
                {
                    return new JSONError(e.Message);
                }

                // Set
                RIPInterfaces.UpdateTimer = UpdateTimer;
                RIPEntryTimers.InvalidTimer = InvalidTimer;
                RIPEntryTimers.HoldTimer = HoldTimer;
                RIPEntryTimers.FlushTimer = FlushTimer;
            }

            var obj = new JSONObject();
            obj.Push("update_timer", RIPInterfaces.UpdateTimer.TotalSeconds);
            obj.Push("invalid_timer", RIPEntryTimers.InvalidTimer.TotalSeconds);
            obj.Push("hold_timer", RIPEntryTimers.HoldTimer.TotalSeconds);
            obj.Push("flush_timer", RIPEntryTimers.FlushTimer.TotalSeconds);
            return obj;
        }

        /*
        public JSON AddInterface(string Data = null)
        {
            try
            {
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Data);
                RIPInterfaces.Add(Interface);

                return new JSONObject("running", Interface.Running);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }
        }
        */

        public JSON Interfaces(string Data = null)
        {
            if(Data != "available" && Data != "started")
            {
                return new JSONError("Expected 'available' or 'active'.");
            }

            var arr = new JSONArray();

            List<Interface> Interfaces = null;
            if (Data == "active")
            {
                Interfaces = RIPInterfaces.GetActiveInterfaces();
            }

            if (Data == "available")
            {
                Interfaces = RIPInterfaces.GetAvailableInterfaces();
            }

            foreach (var Interface in Interfaces)
            {
                arr.Push(Interface.ID);
            }

            return arr;
        }

        public JSON Table(string Data = null)
        {
            var obj = new JSONObject();

            var Rows = RIPTable.GetEntries();
            foreach (var Row in Rows)
            {
                obj.Push(Row.ToString(), RIPEntry(Row));
            }

            return obj;
        }
    }
}
