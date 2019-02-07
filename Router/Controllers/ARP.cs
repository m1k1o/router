using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.Controllers
{
    class ARP
    {
        static private readonly ARPTable ARPTable = ARPTable.Instance;

        public JSON Settings(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 4)
                {
                    return new JSONError("Expected ProxyEnabled, TTL, Timeout, Interval.");
                }

                bool ProxyEnabled;
                TimeSpan CacheTimeout;
                TimeSpan Timeout;
                TimeSpan Interval;
                try
                {
                    ProxyEnabled = Rows[0] == "true";
                    CacheTimeout = TimeSpan.FromSeconds(Int32.Parse(Rows[1]));
                    Timeout = TimeSpan.FromMilliseconds(Int32.Parse(Rows[2]));
                    Interval = TimeSpan.FromMilliseconds(Int32.Parse(Rows[3]));
                }
                catch (Exception e)
                {
                    return new JSONError(e.Message);
                }

                // Set
                Router.ARP.ProxyEnabled = ProxyEnabled;
                ARPEntry.CacheTimeout = CacheTimeout;
                Router.ARP.Timeout = Timeout;
                Router.ARP.Interval = Interval;
            }

            var obj = new JSONObject();
            obj.Push("proxy_enabled", Router.ARP.ProxyEnabled);
            obj.Push("cache_timeout", ARPEntry.CacheTimeout.TotalSeconds);
            obj.Push("timeout", Router.ARP.Timeout.TotalMilliseconds);
            obj.Push("interval", Router.ARP.Interval.TotalMilliseconds);
            return obj;
        }
        
        public JSON Lookup(string Data)
        {
            var Rows = Data.Split('\n');

            // Validate
            if (Rows.Length != 2)
            {
                return new JSONError("Expected InterfaceID, IPAddress.");
            }

            Interface Interface;
            IPAddress IPAddress;
            try
            {
                Interface = Router.Interfaces.Instance.GetInterfaceById(Rows[0]);
                IPAddress = IPAddress.Parse(Rows[1]);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            if (!Interface.Running)
            {
                return new JSONError("Interface must be running.");
            }

            // Action
            var MAC = Router.ARP.Lookup(IPAddress, Interface);

            // Answer
            return new JSONObject("mac", MAC);
        }

        public JSON Table(string Data = null)
        {
            var arr = new JSONArray();
            var obj = new JSONObject();

            var Rows = ARPTable.GetEntries();
            foreach (var Row in Rows)
            {
                if (Row.HasExpired)
                {
                    continue;
                }

                obj.Empty();

                obj.Push("ip", Row.IPAddress);
                obj.Push("mac", Row.PhysicalAddress);
                obj.Push("cache_timeout", Row.ExpiresIn);

                arr.Push(obj);
            }

            return arr;
        }
    }
}
