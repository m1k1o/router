using Router.Helpers;
using System;
using System.Net;
using Router.ARP;

namespace Router.Controllers
{
    static class ARP
    {
        private static readonly ARPTable ARPTable = ARPTable.Instance;

        private static object ARPEntry(ARPEntry ARPEntry)
        {
            return new
            {
                id = ARPEntry.ID,
                ip = ARPEntry.IPAddress,
                mac = ARPEntry.PhysicalAddress,
                cache_timeout = ARPEntry.ExpiresIn,
            };
        }

        public static object Timers(string Request = null)
        {
            var Definition = new
            {
                cache_timeout = Router.ARP.ARPEntry.CacheTimeout.TotalSeconds,
                request_timeout = ARPMiddleware.RequestTimeout.TotalMilliseconds,
                request_interval = ARPMiddleware.RequestInterval.TotalMilliseconds
            };

            if (!string.IsNullOrEmpty(Request))
            {
                try
                {
                    var Response = JSON.DeserializeObject(Request, Definition);

                    var CacheTimeout = TimeSpan.FromSeconds(Response.cache_timeout);
                    var RequestTimeout = TimeSpan.FromMilliseconds(Response.request_timeout);
                    var RequestInterval = TimeSpan.FromMilliseconds(Response.request_interval);
                    
                    Router.ARP.ARPEntry.CacheTimeout = CacheTimeout;
                    ARPMiddleware.RequestTimeout = RequestTimeout;
                    ARPMiddleware.RequestInterval = RequestInterval;
                }
                catch (Exception e)
                {
                    return JSON.Error(e.Message);
                }
            }

            return Definition;
        }

        public static object Proxy(string Request = null)
        {
            var Definition = new
            {
                enabled = ARPMiddleware.ProxyEnabled
            };

            if (!string.IsNullOrEmpty(Request))
            {
                var json = JSON.DeserializeObject(Request, Definition);

                ARPMiddleware.ProxyEnabled = json.enabled;
            }

            return Definition;
        }

        public static object Lookup(string Data)
        {
            try
            {
                var json = JSON.DeserializeObject(Data, new
                {
                    iface = (Interface)null,
                    ip = (IPAddress)null
                });

                if (!json.iface.Running)
                {
                    throw new Exception("Interface must be running.");
                }

                return new {
                    mac = ARPMiddleware.Lookup(json.ip, json.iface)
                };
            }
            catch (Exception e)
            {
                return JSON.Error(e.Message);
            }
        }

        public static object Flush(string Data = null)
        {
            ARPTable.Flush();

            return new
            {
                success = true
            };
        }

        public static object Table(string Data = null)
        {
            dynamic obj = new object {};

            var Rows = ARPTable.GetEntries();
            foreach (var Row in Rows)
            {
                if (Row.HasExpired)
                {
                    continue;
                }

                obj[Row.ID.ToString()] = ARPEntry(Row);
            }

            return obj;
        }

        public static object Initialize(string Data = null)
        {
            return new
            {
                table = Table(),
                timers = Timers(),
                proxy = Proxy()
            };
        }
    }
}
