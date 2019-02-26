using System;
using System.Net;

namespace Router.Controllers.Routing
{
    class Lookup : Controller, Executable
    {
        private RoutingEntry BestMatch;

        public IPAddress IP { private get; set; }

        public void Execute()
        {
            if (IP == null || IP == null)
            {
                throw new Exception("Expected IP.");
            }

            BestMatch = RoutingTable.Instance.Lookup(IP);
        }

        public object Export()
        {
            if (BestMatch != null)
            {
                return Table.Entry(BestMatch);
            }

            return new
            {
                found = false
            };
        }
    }
}
