using Router.Helpers;
using Router.Protocols;
using System.Collections.Generic;
using System.Net;

namespace Router.Packets
{
    sealed class RIP : GeneratorPacket
    {
        public RIPCommandType CommandType { get; set; }

        public byte Version { get; set; }

        public RIP() { }

        public List<RIPRoute> Routes { get; set; } = new List<RIPRoute>();

        public void AddRoute(IPNetwork IPNetwork, IPAddress NextHop, uint Metric)
        {
            Routes.Add(new RIPRoute()
            {
                IPNetwork = IPNetwork,
                NextHop = NextHop,
                Metric = Metric
            });
        }

        public override byte[] Export()
        {
            // Export Routes
            var RouteCollection = new RIPRouteCollection();
            foreach (var Route in Routes)
            {
                RouteCollection.Add(Route.Export());
            }

            var RIPPacket = new RIPPacket(CommandType, RouteCollection)
            {
                Version = Version
            };

            return RIPPacket.Bytes;
        }

        public override void Import(byte[] Bytes)
        {
            var RIPPacket = new RIPPacket(Bytes);

            CommandType = RIPPacket.CommandType;
            Version = RIPPacket.Version;

            // Import Routes
            var RouteCollection = new RIPRouteCollection();
            foreach (var RIPRoute in RIPPacket.RouteCollection)
            {
                var NewRoute = new RIPRoute();
                NewRoute.Import(RIPRoute);
                Routes.Add(NewRoute);
            }
        }
    }

    class RIPRoute
    {
        public ushort AFI { get; set; } = 2;

        public ushort RouteTag { get; set; } = 0;

        public IPAddress IP { get; set; }

        public IPSubnetMask Mask { get; set; }

        public IPAddress NextHop { get; set; }

        public uint Metric { get; set; }

        public IPNetwork IPNetwork
        {
            private get => IPNetwork.Parse(IP, Mask);
            set
            {
                IP = value.NetworkAddress;
                Mask = value.SubnetMask;
            }
        }

        public Protocols.RIPRoute Export() => new Protocols.RIPRoute(IPNetwork, NextHop, Metric)
        {
            AddressFamilyIdentifier = AFI,
            RouteTag = RouteTag
        };

        public void Import(Protocols.RIPRoute RIPRoute)
        {
            AFI = RIPRoute.AddressFamilyIdentifier;
            RouteTag = RIPRoute.RouteTag;
            IPNetwork = RIPRoute.IPNetwork;
            NextHop = RIPRoute.NextHop;
            Metric = RIPRoute.Metric;
        }
    }
}
