using Router.Helpers;
using System;
using System.Net;
using Router.ARP;

namespace Router.Controllers
{
    static class ARP
    {
        private static readonly ARPTable ARPTable = ARPTable.Instance;

        private static JSON ARPEntry(ARPEntry ARPEntry)
        {
            var obj = new JSONObject();
            //obj.Push("id", ARPEntry.ID);
            obj.Push("ip", ARPEntry.IPAddress);
            obj.Push("mac", ARPEntry.PhysicalAddress);
            obj.Push("cache_timeout", ARPEntry.ExpiresIn);
            return obj;
        }

        public static JSON Timers(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 3)
                {
                    return new JSONError("Expected CacheTimeout, RequestTimeout, RequestInterval.");
                }
                
                TimeSpan CacheTimeout;
                TimeSpan RequestTimeout;
                TimeSpan RequestInterval;
                try
                {
                    CacheTimeout = TimeSpan.FromSeconds(Int32.Parse(Rows[0]));
                    RequestTimeout = TimeSpan.FromMilliseconds(Int32.Parse(Rows[1]));
                    RequestInterval = TimeSpan.FromMilliseconds(Int32.Parse(Rows[2]));
                }
                catch (Exception e)
                {
                    return new JSONError(e.Message);
                }

                // Set
                Router.ARP.ARPEntry.CacheTimeout = CacheTimeout;
                ARPMiddleware.RequestTimeout = RequestTimeout;
                ARPMiddleware.RequestInterval = RequestInterval;
            }

            var obj = new JSONObject();
            obj.Push("cache_timeout", Router.ARP.ARPEntry.CacheTimeout.TotalSeconds);
            obj.Push("request_timeout", ARPMiddleware.RequestTimeout.TotalMilliseconds);
            obj.Push("request_interval", ARPMiddleware.RequestInterval.TotalMilliseconds);
            return obj;
        }

        public static JSON Proxy(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 1)
                {
                    return new JSONError("Expected ProxyEnabled.");
                }

                ARPMiddleware.ProxyEnabled = Rows[0] == "true";
            }

            var obj = new JSONObject();
            obj.Push("enabled", ARPMiddleware.ProxyEnabled);
            return obj;
        }

        public static JSON Lookup(string Data)
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
            var MAC = ARPMiddleware.Lookup(IPAddress, Interface);

            // Answer
            return new JSONObject("mac", MAC);
        }

        public static JSON Table(string Data = null)
        {
            var obj = new JSONObject();

            var Rows = ARPTable.GetEntries();
            foreach (var Row in Rows)
            {
                if (Row.HasExpired)
                {
                    continue;
                }

                obj.Push(Row.ID.ToString(), ARPEntry(Row));
            }

            return obj;
        }

        public static JSON Initialize(string Data = null)
        {
            var obj = new JSONObject();
            obj.Push("table", Table());
            obj.Push("timers", Timers());
            obj.Push("proxy", Proxy());
            return obj;
        }
    }
}
