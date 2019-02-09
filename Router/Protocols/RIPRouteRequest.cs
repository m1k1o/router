using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.Protocols
{
    class RIPRouteRequest : RIPRoute
    {
        public RIPRouteRequest() : base()
        {
            AddressFamilyIdentifier = 0;
            Metric = 16;
        }
    }
}
