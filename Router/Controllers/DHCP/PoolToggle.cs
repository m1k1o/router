using Router.DHCP;
using System;

namespace Router.Controllers.DHCP
{
    class PoolToggle : Controller, Executable
    {
        public bool IsDynamic { get; private set; }

        public Interface Interface { private get; set; }

        public void Execute()
        {
            if (Interface == null)
            {
                throw new Exception("Expected Interface.");
            }

            if (!DHCPPool.Interfaces.ContainsKey(Interface))
            {
                throw new Exception("Pool not available.");
            }

            var EPool = DHCPPool.Interfaces[Interface];
            EPool.IsDynamic = !EPool.IsDynamic;
            IsDynamic = EPool.IsDynamic;
        }

        public object Export() => this;
    }
}
