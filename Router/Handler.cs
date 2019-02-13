using PacketDotNet;
using SharpPcap;
using System;

namespace Router
{
    class Handler
    {
        public Interface Interface { get; private set; }

        public RawCapture RawCapture { get; private set; }
        public InternetLinkLayerPacket InternetLinkLayerPacket { get; private set; }
        public InternetPacket InternetPacket { get; private set; }
        public TransportPacket TransportPacket { get; private set; }
        public ApplicationPacket ApplicationPacket { get; private set; }

        public int Layer { get; private set; } = 0;

        public Handler(RawCapture RawCapture, Interface Interface)
        {
            this.RawCapture = RawCapture;
            this.Interface = Interface;

            Parse();
        }

        private void Parse()
        {
            if (!LinkLayer())
            {
                Layer = 1;
                throw new Exception("Failed to parse Link Layer");
            }

            if(!(InternetLinkLayerPacket is EthernetPacket))
            {
                throw new Exception("Packet is not Ethernet");
            }

            if (!InternetLayer())
            {
                Layer = 2;
                return;
            }

            if (!TransportLayer())
            {
                Layer = 3;
                return;
            }

            if (!ApplicationLayer())
            {
                Layer = 4;
                return;
            }

            Layer = 5;
            return;
        }

        private bool LinkLayer()
        {
            InternetLinkLayerPacket = (InternetLinkLayerPacket)Packet.ParsePacket(RawCapture.LinkLayerType, RawCapture.Data);
            return InternetLinkLayerPacket != null;
        }

        private bool InternetLayer()
        {
            InternetPacket = (InternetPacket)InternetLinkLayerPacket.Extract(typeof(InternetPacket));
            return InternetPacket != null;
        }

        private bool TransportLayer()
        {
            TransportPacket = (TransportPacket)InternetPacket.Extract(typeof(TransportPacket));
            return TransportPacket != null;
        }

        private bool ApplicationLayer()
        {
            ApplicationPacket = (ApplicationPacket)TransportPacket.Extract(typeof(ApplicationPacket));
            return TransportPacket != null;
        }

        public bool IsFromMe
            => Equals(((EthernetPacket)InternetLinkLayerPacket).SourceHwAddress, Interface.PhysicalAddress);

        public bool CheckEtherType(EthernetPacketType EthernetPacketType)
        {
            if (IsFromMe)
            {
                return false;
            }

            return EthernetPacket.Type == EthernetPacketType;
        }

        // Shortcuts - most common protocols

        public EthernetPacket EthernetPacket => (EthernetPacket)InternetLinkLayerPacket;

        public IPv4Packet IPv4Packet => (IPv4Packet)InternetPacket;

        public UdpPacket UdpPacket => (UdpPacket)TransportPacket;

        public TcpPacket TcpPacket => (TcpPacket)TransportPacket;
    }
}
