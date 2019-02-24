using Router.Helpers;
using Router.Protocols;
using System.Net;

namespace Router.Packets
{
    class RIP : UDP, Generator
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

        public new byte[] Export()
        {
            var RIPPacket = new RIPPacket(CommandType, RouteCollection)
            {
                Version = Version
            };

            return RIPPacket.Bytes;
        }

        public new byte[] ExportAll()
        {
            base.Payload = Export();
            return base.ExportAll();
        }

        public new void Import(byte[] Bytes)
        {
            var RIPPacket = new RIPPacket(Bytes);

            CommandType = RIPPacket.CommandType;
            Version = RIPPacket.Version;
            RouteCollection = RIPPacket.RouteCollection;
        }

        public new void ImportAll(byte[] Bytes)
        {
            base.ImportAll(Bytes);
            Import(Payload);
        }

        /*
        public new void Parse(string[] Rows, ref int i)
        {
            // Parse UDP
            base.Parse(Rows, ref i);

            // Parse RIP
            if (Rows.Length - i < 3 && (Rows.Length - i - 2) % 6 != 0)
            {
                throw new Exception("Expected CommandType, Version, [AddressFamilyIdentifier, RouteTag, Network, SubnetMask, NextHop, Metric]+.");
            }

            CommandType = (RIPCommandType)Convert.ToByte(Rows[i++]);
            Version = Convert.ToByte(Rows[i++]);

            while (i < Rows.Length - 1)
            {
                AddRoute(
                    UInt16.Parse(Rows[i++].Or("0")), // AddressFamilyIdentifier
                    UInt16.Parse(Rows[i++].Or("0")), // RouteTag
                    IPNetwork.Parse(Rows[i++].Or("0.0.0.0"), Rows[i++].Or("0.0.0.0")), // Network
                    IPAddress.Parse(Rows[i++].Or("0.0.0.0")), // NextHop
                    UInt32.Parse(Rows[i++].Or("0")) // Metric
                );
            }
        }
        */
    }
}
