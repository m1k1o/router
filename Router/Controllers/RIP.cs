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

                    RIPInterfaces.UpdateTimer = UpdateTimer;
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
            obj.Push("update_timer", RIPInterfaces.UpdateTimer.TotalSeconds);
            obj.Push("invalid_timer", RIPEntryTimers.InvalidTimer.TotalSeconds);
            obj.Push("hold_timer", RIPEntryTimers.HoldTimer.TotalSeconds);
            obj.Push("flush_timer", RIPEntryTimers.FlushTimer.TotalSeconds);
            return obj;
        }

        public static JSON Updates(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                if(Data == "start")
                {
                    RIPInterfaces.ActiveUpdates = true;
                }

                if (Data == "stop")
                {
                    RIPInterfaces.ActiveUpdates = false;
                }
            }

            return new JSONObject("active", RIPInterfaces.ActiveUpdates);
        }

        public static JSON InterfaceToggle(string Data)
        {
            try
            {
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Data);
                if (RIPInterfaces.IsActive(Interface))
                {
                    RIPInterfaces.Remove(Interface);
                }
                else
                {
                    RIPInterfaces.Add(Interface);
                }

                var obj = new JSONObject();
                obj.Push("active", RIPInterfaces.IsActive(Interface));
                obj.Push("running", RIPInterfaces.IsRunning(Interface));
                return obj;
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }
        }
        
        public static JSON Interfaces(string Data = null)
        {
            var obj = new JSONObject();
            var obj2 = new JSONObject();

            var Interfaces = Router.Interfaces.Instance.GetInteraces();
            foreach (var Interface in Interfaces)
            {
                obj2.Empty();
                obj2.Push("active", RIPInterfaces.IsActive(Interface));
                obj2.Push("running", RIPInterfaces.IsRunning(Interface));

                obj.Push(Interface.ID.ToString(), obj2);
            }

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
            obj.Push("updates", Updates());
            obj.Push("interfaces", Interfaces());
            obj.Push("timers", Timers());
            return obj;
        }
    }
}
