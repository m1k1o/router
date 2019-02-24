using Router.Helpers;
using System;
using System.Net;

namespace Router.Controllers.Routing
{
    class RemoveStatic : Controller, Executable
    {
        public IPAddress IP { get; set; }
        public IPSubnetMask Mask { get; set; }

        public void Execute()
        {
            if (IP == null || Mask == null)
            {
                throw new Exception("Expected IP and Mask.");
            }

            var Network = IPNetwork.Parse(IP, Mask);
            RoutingTable.Instance.Remove(Network, ADistance.Static);
        }

        public object Export() => this;
    }
}
