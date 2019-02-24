using Router.DHCP;
using System;
using System.Net.NetworkInformation;

namespace Router.Controllers.DHCP
{
    class RemoveStatic : Controller, Executable
    {
        public PhysicalAddress MAC { get; set; }
        public Interface Interface { get; set; }

        public void Execute()
        {
            if (MAC == null || Interface == null)
            {
                throw new Exception("Expected IP and Interface.");
            }

            DHCPTable.Instance.RemoveStatic(MAC, Interface);
        }

        public object Export() => this;
    }
}
