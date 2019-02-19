using Router.Helpers;
using Router.Protocols;
using System;
using System.Linq;
using System.Net;

namespace Router.Generator
{
    class RIP : UDP
    {
        public RIPCommandType CommandType { get; set; }

        public byte Version { get; set; }

        public RIP() { }

        private RIPRouteCollection RouteCollection = new RIPRouteCollection();

        public void AddRoute(ushort AddressFamilyIdentifier, ushort RouteTag, IPNetwork IPNetwork, IPAddress NextHop, uint Metric)
        {
            var Route = new RIPRoute(IPNetwork, NextHop, Metric)
            {
                AddressFamilyIdentifier = AddressFamilyIdentifier,
                RouteTag = RouteTag
            };

            RouteCollection.Add(Route);
        }

        public void AddRoute(IPNetwork IPNetwork, IPAddress NextHop, uint Metric)
        {
            RouteCollection.Add(new RIPRoute(IPNetwork, NextHop, Metric));
        }

        public override PacketDotNet.Packet Export()
        {
            var RIPPacket = new RIPPacket(CommandType, RouteCollection)
            {
                Version = Version
            };
            base.Payload = RIPPacket.Bytes;
            return base.Export();
        }

        public override void Parse(string Data)
        {
            // Parse UDP
            base.Parse(Data);

            var Rows = Data.Split('\n').Skip(6).ToArray();
            if (Rows.Length < 3 && (Rows.Length - 2) % 6 != 0)
            {
                throw new Exception("Expected CommandType, Version, [AddressFamilyIdentifier, RouteTag, Network, SubnetMask, NextHop, Metric]+.");
            }

            var i = 0;
            CommandType = (RIPCommandType)Convert.ToByte(Rows[i++]);
            Version = Convert.ToByte(Rows[i++]);

            do
            {
                AddRoute(
                    UInt16.Parse(Rows[i++]), // AddressFamilyIdentifier
                    UInt16.Parse(Rows[i++]), // RouteTag
                    IPNetwork.Parse(Rows[i++], Rows[i++]), // Network
                    IPAddress.Parse(Rows[i++]), // NextHop
                    UInt32.Parse(Rows[i++]) // Metric
                );
            }
            while (i < Rows.Length - 1);
        }
    }
}
