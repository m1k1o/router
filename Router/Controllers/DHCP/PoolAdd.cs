using Router.DHCP;
using System;
using System.Collections.Generic;
using System.Net;

namespace Router.Controllers.DHCP
{
    class PoolAdd : Controller, Executable
    {
        private DHCPPool Entry;

        public Interface Interface { get; set; }
        public IPAddress FirstIP { get; set; }
        public IPAddress LastIP { get; set; }
        public bool IsDynamic { get; set; } = true;

        public void Execute()
        {
            if (Interface == null || FirstIP == null || LastIP == null)
            {
                throw new Exception("Expected Interface, FirstIP, LastIP.");
            }

            Entry = new DHCPPool(FirstIP, LastIP)
            {
                IsDynamic = IsDynamic
            };

            DHCPPool.Interfaces.Add(Interface, Entry);
        }

        public object Export() => new Dictionary<string, object>()
        {
            { Interface.ID.ToString(), Pools.Entry(Entry) }
        };
    }
}
