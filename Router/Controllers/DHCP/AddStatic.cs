using Router.DHCP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Controllers.DHCP
{
    class AddStatic : Controller, Executable
    {
        private DHCPLease Entry;

        public PhysicalAddress MAC { get; set; }
        public Interface Interface { get; set; }
        public IPAddress IP { get; set; }

        public void Execute()
        {
            if (MAC == null || Interface == null || IP == null)
            {
                throw new Exception("Expected MAC, Interface, IP.");
            }

            Entry = DHCPTable.Instance.AddStatic(MAC, Interface, IP);
        }

        public object Export() => new Dictionary<string, object>()
        {
            { Entry.ID.ToString(), Table.Entry(Entry) }
        };
    }
}
