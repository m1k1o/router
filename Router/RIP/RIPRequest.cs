using Router.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Router.RIP
{
    class RIPRequest
    {
        public PhysicalAddress SrcMac { get; private set; }
        public IPAddress SrcIP { get; private set; }
        public ushort SrcPort { get; private set; }
        public Interface Interface { get; private set; }
        public RIPRouteCollection RouteCollection { get; private set; }

        public static void OnReceived(RIPRequest RIPRequest)
        {
            RIPRequest.SendResponse();
        }


        public RIPRequest(PhysicalAddress SrcMac, IPAddress SrcIP, ushort SrcPort, Interface Interface, RIPRouteCollection RouteCollection)
        {
            this.SrcMac = SrcMac;
            this.SrcIP = SrcIP;
            this.SrcPort = SrcPort;
            this.Interface = Interface;
            this.RouteCollection = RouteCollection;
        }

        public void SendResponse()
        {
            InsertRoutes();

            var RIPResponse = new RIPResponse(Interface);
            RIPResponse.Send(this);
        }

        private void InsertRoutes()
        {
            foreach (var Route in RouteCollection)
            {
                var FoundNetworks = RIPTable.Instance.FindAll(Route.IPNetwork);
                if(FoundNetworks != null)
                {
                    var BestRoute = FoundNetworks.BestRoute();

                    Route.NextHop = Interface.IPAddress;
                    if (Interface.IsReachable(BestRoute.NextHopIP))
                    {
                        Route.NextHop = BestRoute.NextHopIP;
                    }

                    Route.Metric = BestRoute.Metric;
                }
            }
        }
    }
}
