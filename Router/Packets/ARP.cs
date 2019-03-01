using PacketDotNet;
using PacketDotNet.Utils;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Packets
{
    sealed class ARP : GeneratorPacket
    {
        public ARPOperation Operation { get; set; }
        public PhysicalAddress SenderHardwareAddress { get; set; }
        public IPAddress SenderProtocolAddress { get; set; }
        public PhysicalAddress TargetHardwareAddress { get; set; }
        public IPAddress TargetProtocolAddress { get; set; }

        public ARP() { }

        public override byte[] Export()
        {
            return new ARPPacket(Operation, TargetHardwareAddress, TargetProtocolAddress, SenderHardwareAddress, SenderProtocolAddress).Bytes;
        }

        public override void Import(byte[] Bytes)
        {
            if (Bytes == null) return;

            var ARPPacket = new ARPPacket(new ByteArraySegment(Bytes));

            Operation = ARPPacket.Operation;
            TargetHardwareAddress = ARPPacket.TargetHardwareAddress;
            TargetProtocolAddress = ARPPacket.TargetProtocolAddress;
            SenderHardwareAddress = ARPPacket.SenderHardwareAddress;
            SenderProtocolAddress = ARPPacket.SenderProtocolAddress;
        }
    }
}
