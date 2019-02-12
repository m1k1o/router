using PacketDotNet.LLDP;
using System.Net.NetworkInformation;
using PacketDotNet;

namespace Router.LLDP
{
    static class LLDPResponse
    {
        public static PhysicalAddress LLDPMulticast = PhysicalAddress.Parse("01-80-C2-00-00-0E");
        public static ushort TimeToLive = 120;

        public static string SystemName = "WAN Router";
        public static string SystemDescription = "Lightweight C# Router";

        public static void Send(Interface Interface)
        {
            var TlvCollection = new TLVCollection
            {
                //new ChassisID(ChassisSubTypes.NetworkAddress, Interface.IPAddress),
                new ChassisID(ChassisSubTypes.MACAddress, Interface.PhysicalAddress),
                new PortID(PortSubTypes.MACAddress, Interface.PhysicalAddress),
                new TimeToLive(TimeToLive),

                new PortDescription(Interface.FriendlyName),
                new SystemName(SystemName),
                new SystemDescription(SystemDescription),

                new SystemCapabilities((ushort)CapabilityOptions.Router, (ushort)CapabilityOptions.Router),

                new EndOfLLDPDU()
            };

            var LLDPPacket = new LLDPPacket
            {
                TlvCollection = TlvCollection
            };

            var EthernetPacket = new EthernetPacket(Interface.PhysicalAddress, LLDPMulticast, EthernetPacketType.LLDP)
            {
                PayloadPacket = new LLDPPacket(new PacketDotNet.Utils.ByteArraySegment(LLDPPacket.Bytes))
            };

            Interface.SendPacket(EthernetPacket.Bytes);
        }

        public static void OnReceived(Interface Interface, LLDPPacket LLDPPacket)
        {
            LLDPTable.Push(LLDPPacket.TlvCollection, Interface);
        }
    }
}
