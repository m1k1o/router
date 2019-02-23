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

        public RIPRequest(PhysicalAddress SrcMac, IPAddress SrcIP, ushort SrcPort, RIPRouteCollection RouteCollection)
        {
            this.SrcMac = SrcMac;
            this.SrcIP = SrcIP;
            this.SrcPort = SrcPort;
            this.RouteCollection = RouteCollection;
        }

        public void SendResponse(Interface Interface)
        {
            var RIPResponse = new RIPResponse(Interface);
            RIPResponse.Send(this);
        }

        public bool AskingForUpdate
            => RouteCollection.Count == 1 && RouteCollection[0].AddressFamilyIdentifier == 0 && RouteCollection[0].Metric ==16;

        public void Export(Interface Interface)
        {
            foreach (var Route in RouteCollection)
            {
                var FoundNetworks = RIPTable.Instance.FindAll(Route.IPNetwork);
                if (FoundNetworks.Count > 0)
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

        public static void AskForUpdate(Interface Interface)
        {
            var RouteCollection = new RIPRouteCollection
            {
                new RIPRouteRequest()
            };

            Protocols.RIP.Send(RIPCommandType.Request, RouteCollection, Interface);
        }

        public static void OnReceived(PhysicalAddress SrcMac, IPAddress SrcIP, ushort SrcPort, RIPRouteCollection RouteCollection, Interface Interface)
        {
            var RIPRequest = new RIPRequest(SrcMac, SrcIP, SrcPort, RouteCollection);
            RIPRequest.SendResponse(Interface);
        }
    }
}
