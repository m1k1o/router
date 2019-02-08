using Router.Protocols;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.RIP
{
    class RIPRequest
    {
        public PhysicalAddress SrcMac { get; private set; }
        public IPAddress SrcIP { get; private set; }
        public ushort SrcPort { get; private set; }
        public RIPRouteCollection RouteCollection { get; private set; }
        public Interface Interface { get; private set; }

        public RIPRequest(PhysicalAddress SrcMac, IPAddress SrcIP, ushort SrcPort, RIPRouteCollection RouteCollection, Interface Interface)
        {
            this.SrcMac = SrcMac;
            this.SrcIP = SrcIP;
            this.SrcPort = SrcPort;
            this.RouteCollection = RouteCollection;
            this.Interface = Interface;
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

        static public void OnReceived(PhysicalAddress SrcMac, IPAddress SrcIP, ushort SrcPort, RIPRouteCollection RouteCollection, Interface Interface)
        {
            var RIPRequest = new RIPRequest(SrcMac, SrcIP, SrcPort, RouteCollection, Interface);
            RIPRequest.SendResponse();
        }
    }
}
