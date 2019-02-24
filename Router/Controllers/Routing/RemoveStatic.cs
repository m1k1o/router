using Router.Helpers;
using System;
using System.Net;

namespace Router.Controllers.Routing
{
    class RemoveStatic : Controller, Executable
    {
        public string ID { get; private set; }

        public IPAddress IP;
        public IPSubnetMask Mask;

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
