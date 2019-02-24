﻿using PacketDotNet;
using PacketDotNet.Utils;
using System.Net;

namespace Router.Packets
{
    sealed class IP : PacketsImportExport, PacketsPayloadData
    {
        public static EthernetPacketType EthernetPacketType = EthernetPacketType.IpV4;

        public IPAddress SourceAddress { get; set; }
        public IPAddress DestinationAddress { get; set; }
        public int TimeToLive { get; set; }

        public IPProtocolType IPProtocolType { get; set; }
        public byte[] Payload { get; set; }

        public void PayloadData(byte[] Data) => Payload = Data;

        public IP() { }

        public byte[] Export()
        {
            var IPv4Packet = new IPv4Packet(SourceAddress, DestinationAddress)
            {
                TimeToLive = TimeToLive,
                Protocol = IPProtocolType
            };

            if (Payload != null)
            {
                IPv4Packet.PayloadData = Payload;
            }

            IPv4Packet.Checksum = IPv4Packet.CalculateIPChecksum();
            return IPv4Packet.Bytes;
        }

        public void Import(byte[] Bytes)
        {
            var IPv4Packet = new IPv4Packet(new ByteArraySegment(Bytes));

            SourceAddress = IPv4Packet.SourceAddress;
            DestinationAddress = IPv4Packet.DestinationAddress;
            TimeToLive = IPv4Packet.TimeToLive;
            IPProtocolType = IPv4Packet.Protocol;
            Payload = IPv4Packet.PayloadData;
        }
    }
}
