using Router.Helpers;
using Router.RIP;
using System;
using System.Collections.Generic;

namespace Router.Controllers
{
    class RIP
    {
        private static readonly RIPTable RIPTable = RIPTable.Instance;

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
            obj.Push("can_be_updated", RIPEntry.CanBeUpdated);
            obj.Push("timers_enabled", RIPEntry.TimersEnabled);
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

        public JSON Updates(string Data = null)
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

        public JSON AddInterface(string Data)
        {
            try
            {
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Data);
                RIPInterfaces.Add(Interface);

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

        public JSON RemoveInterface(string Data)
        {
            try
            {
                var Interface = Router.Interfaces.Instance.GetInterfaceById(Data);
                RIPInterfaces.Remove(Interface);

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

        public JSON Interfaces(string Data = null)
        {
            var obj = new JSONObject();
            var obj2 = new JSONObject();

            var Interfaces = Router.Interfaces.Instance.GetInteraces();
            foreach (var Interface in Interfaces)
            {
                obj2.Empty();
                obj2.Push("active", RIPInterfaces.IsActive(Interface));
                obj2.Push("running", RIPInterfaces.IsRunning(Interface));

                obj.Push(Interface.ToString(), obj2);
            }

            return obj;
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
