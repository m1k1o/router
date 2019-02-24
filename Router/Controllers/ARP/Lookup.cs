using Router.ARP;
using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Controllers.ARP
{
    class Lookup : Controller, Executable
    {
        public Interface Interface { private get; set; }
        public IPAddress IP { private get; set; }
        public PhysicalAddress MAC { get; private set; }

        public void Execute()
        {
            if (Interface == null || IP == null)
            {
                throw new Exception("Expected Interface and IP.");
            }

            if (!Interface.Running)
            {
                throw new Exception("Interface must be running.");
            }

            MAC = ARPMiddleware.Lookup(IP, Interface);
        }

        public object Export() => this;
    }
}
