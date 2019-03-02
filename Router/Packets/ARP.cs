using PacketDotNet;
using PacketDotNet.Utils;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Packets
{
    // TODO: Payload
    sealed class ARP : GeneratorPacket
    {
        public ARPOperation Operation { get; set; } = 0;
        public PhysicalAddress SenderHardwareAddress { get; set; } = PhysicalAddress.Parse("00-00-00-00-00-00");
        public IPAddress SenderProtocolAddress { get; set; } = IPAddress.Parse("0.0.0.0");
        public PhysicalAddress TargetHardwareAddress { get; set; } = PhysicalAddress.Parse("00-00-00-00-00-00");
        public IPAddress TargetProtocolAddress { get; set; } = IPAddress.Parse("0.0.0.0");

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
