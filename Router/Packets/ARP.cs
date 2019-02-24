using PacketDotNet;
using PacketDotNet.Utils;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Packets
{
    sealed class ARP : Ethernet, Generator
    {
        public ARPOperation Operation { get; set; }
        public PhysicalAddress SenderHardwareAddress { get; set; }
        public IPAddress SenderProtocolAddress { get; set; }
        public PhysicalAddress TargetHardwareAddress { get; set; }
        public IPAddress TargetProtocolAddress { get; set; }

        public ARP() { }

        public new byte[] Export()
        {
            return new ARPPacket(Operation, TargetHardwareAddress, TargetProtocolAddress, SenderHardwareAddress, SenderProtocolAddress).Bytes;
        }

        public new byte[] ExportAll()
        {
            base.EthernetPacketType = EthernetPacketType.Arp;
            base.Payload = Export();
            return base.ExportAll();
        }

        public new void Import(byte[] Bytes)
        {
            var ARPPacket = new ARPPacket(new ByteArraySegment(Bytes));

            Operation = ARPPacket.Operation;
            TargetHardwareAddress = ARPPacket.TargetHardwareAddress;
            TargetProtocolAddress = ARPPacket.TargetProtocolAddress;
            SenderHardwareAddress = ARPPacket.SenderHardwareAddress;
            SenderProtocolAddress = ARPPacket.SenderProtocolAddress;
        }

        public new void ImportAll(byte[] Bytes)
        {
            base.ImportAll(Bytes);
            Import(Payload);
        }

        /*
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
            SenderHardwareAddress = Utilities.ParseMAC(Rows[i++]);
            SenderProtocolAddress = IPAddress.Parse(Rows[i++]);
            TargetHardwareAddress = Utilities.ParseMAC(Rows[i++].Or("00:00:00:00:00:00"));
            TargetProtocolAddress = IPAddress.Parse(Rows[i++].Or("0.0.0.0"));
        }
        */
    }
}
