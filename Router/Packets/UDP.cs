﻿using PacketDotNet;
using PacketDotNet.Utils;

namespace Router.Packets
{
    sealed class UDP : GeneratorPayload
    {
        public ushort SourcePort { get; set; } = 0;
        public ushort DestinationPort { get; set; } = 0;

        public UDP() { }

        public override byte[] Export()
        {
            var UdpPacket = new UdpPacket(SourcePort, DestinationPort);

            if (Payload != null)
            {
                UdpPacket.PayloadData = Payload;
            }

            return UdpPacket.Bytes;
        }

        public override void Import(byte[] Bytes)
        {
            if (Bytes == null) return;

            var UdpPacket = new UdpPacket(new ByteArraySegment(Bytes));

            SourcePort = UdpPacket.SourcePort;
            DestinationPort = UdpPacket.DestinationPort;

            // Auto Types
            if (DestinationPort == 520)
            {
                PayloadPacket = new RIP();
                PayloadPacket.Import(UdpPacket.PayloadData);
            }
            else if (DestinationPort == 67 || DestinationPort == 68)
            {
                PayloadPacket = new DHCP();
                PayloadPacket.Import(UdpPacket.PayloadData);
            }
            else
            {
                PayloadPacket = new Payload();
                PayloadPacket.Import(UdpPacket.PayloadData);
            }
        }
    }
}
