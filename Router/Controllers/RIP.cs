using Router.Helpers;
using Router.RIP;
using System;

namespace Router.Controllers
{
    class RIP
    {
        public JSON Timers(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 3)
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
                    HoldTimer = TimeSpan.FromMilliseconds(Int32.Parse(Rows[2]));
                    FlushTimer = TimeSpan.FromMilliseconds(Int32.Parse(Rows[3]));
                }
                catch (Exception e)
                {
                    return new JSONError(e.Message);
                }

                // Set

                //.UpdateTimer = UpdateTimer;
                throw new NotImplementedException();

                RIPTimers.InvalidTimer = InvalidTimer;
                RIPTimers.HoldTimer = HoldTimer;
                RIPTimers.FlushTimer = FlushTimer;
            }

            var obj = new JSONObject();

            //obj.Push("update_timer", .InvalidTimer.TotalSeconds);
            throw new NotImplementedException();

            obj.Push("invalid_timer", RIPTimers.InvalidTimer.TotalSeconds);
            obj.Push("hold_timer", RIPTimers.HoldTimer.TotalMilliseconds);
            obj.Push("flush_timer", RIPTimers.FlushTimer.TotalMilliseconds);
            return obj;
        }

    }
}
