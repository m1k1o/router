using PacketDotNet;
using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Generator
{
    sealed class ARP : Generator
    {
        public PhysicalAddress SourceHwAddress { get; set; }
        public PhysicalAddress DestinationHwAddress { get; set; }

        public ARPOperation Operation { get; set; }
        public PhysicalAddress SenderHardwareAddress { get; set; }
        public IPAddress SenderProtocolAddress { get; set; }
        public PhysicalAddress TargetHardwareAddress { get; set; }
        public IPAddress TargetProtocolAddress { get; set; }

        public ARP() { }

        public Packet Export()
        {
            var ARPPacket = new ARPPacket(Operation, TargetHardwareAddress, TargetProtocolAddress, SenderHardwareAddress, SenderProtocolAddress);

            var EthernetPacket = new EthernetPacket(SourceHwAddress, DestinationHwAddress, EthernetPacketType.Arp)
            {
                PayloadPacket = ARPPacket
            };

            return EthernetPacket;
        }

        public void Parse(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 7)
            {
                throw new Exception("Expected SourceHwAddress, DestinationHwAddress, Operation, SenderHardwareAddress, SenderProtocolAddress, TargetHardwareAddress, TargetProtocolAddress.");
            }

            var i = 0;
            SourceHwAddress = PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));
            DestinationHwAddress = PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));
            Operation = (ARPOperation)UInt16.Parse(Rows[i++]);
            SenderHardwareAddress = PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));
            SenderProtocolAddress = IPAddress.Parse(Rows[i++]);
            TargetHardwareAddress = PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));
            TargetProtocolAddress = IPAddress.Parse(Rows[i++]);
        }
    }
}
