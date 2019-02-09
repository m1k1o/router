using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.Protocols
{
    class RIPRequestRoute : RIPRoute
    {
        public RIPRequestRoute() : base(null)
        {
            AddressFamilyIdentifier = 0;
            Metric = 16;
        }
    }
}
