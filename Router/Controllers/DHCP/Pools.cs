using Router.DHCP;
using System.Collections.Generic;

namespace Router.Controllers.DHCP
{
    class Pools : Controller
    {
        public static object Entry(DHCPPool DHCPPool) => new
        {
            //id = DHCPPool.ID.ToString(),
            first_ip = DHCPPool.FirstIP,
            last_ip = DHCPPool.LastIP,
            is_dynamic = DHCPPool.IsDynamic,
            available = DHCPPool.Available,
            allocated = DHCPPool.Allocated,
        };

        public object Export()
        {
            var DHCPPools = DHCPPool.Interfaces;

            var Dictionary = new Dictionary<string, object>();
            foreach (KeyValuePair<Interface, DHCPPool> DHCPPool in DHCPPools)
            {
                Dictionary.Add(DHCPPool.Key.ID.ToString(), Entry(DHCPPool.Value));
            }
            return Dictionary;
        }
    }
}