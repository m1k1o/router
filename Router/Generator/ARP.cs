using PacketDotNet;
using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Generator
{
    sealed class ARP : Ethernet, Generator
    {
        public ARPOperation Operation { get; set; }
        public PhysicalAddress SenderHardwareAddress { get; set; }
        public IPAddress SenderProtocolAddress { get; set; }
        public PhysicalAddress TargetHardwareAddress { get; set; }
        public IPAddress TargetProtocolAddress { get; set; }

        public ARP() { }

        public Packet Export()
        {
            // Create ARP
            var ARPPacket = new ARPPacket(Operation, TargetHardwareAddress, TargetProtocolAddress, SenderHardwareAddress, SenderProtocolAddress);

            // Create Ethernet
            return Export(EthernetPacketType.Arp, ARPPacket);
        }

        public new void Parse(string[] Rows, ref int i)
        {
            // Parse Ethernet
            base.Parse(Rows, ref i);

            // Parse ARP
            if (Rows.Length - i != 5)
            {
                throw new Exception("Expected Operation, SenderHardwareAddress, SenderProtocolAddress, TargetHardwareAddress, TargetProtocolAddress.");
            }

            Operation = (ARPOperation)UInt16.Parse(Rows[i++]);
            SenderHardwareAddress = PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));
            SenderProtocolAddress = IPAddress.Parse(Rows[i++]);
            TargetHardwareAddress = PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));
            TargetProtocolAddress = IPAddress.Parse(Rows[i++]);
        }
    }
}
