using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.Controllers
{
    class Global
    {
        public JSON Initialize(string Data)
        {
            var obj = new JSONObject();
            obj.Push("interfaces", (new Interfaces()).Show());
            obj.Push("arp_table", (new ARP()).Table());
            obj.Push("routing_table", (new Routing()).Table());
            return obj;
        }

        public JSON Update(string Data)
        {
            var obj = new JSONObject();
            obj.Push("arp_table", (new ARP()).Table());
            obj.Push("routing_table", (new Routing()).Table());
            return obj;
        }
    }
}
