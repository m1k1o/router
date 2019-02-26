using Router.DHCP;
using System;

namespace Router.Controllers.DHCP
{
    class PoolRemove : Controller, Executable
    {
        public Interface Interface { get; set; }

        public void Execute()
        {
            if (Interface == null)
            {
                throw new Exception("Expected Interface.");
            }

            DHCPPool.Interfaces.Remove(Interface);
        }

        public object Export() => this;
    }
}
