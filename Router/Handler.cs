using PacketDotNet;
using RIP;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router
{
    class Handler
    {
        private Interface Interface;

        public void SetInterface(Interface Interface)
        {
            this.Interface = Interface;
        }

        public void Ethernet(RawCapture Capture)
        {
            var packet = (EthernetPacket)PacketDotNet.Packet.ParsePacket(Capture.LinkLayerType, Capture.Data);
            if (packet == null)
            {
                throw new Exception("Packet is not Ethernet Packet.");
            }

            // My packet?
            if(Equals(packet.SourceHwAddress, Interface.PhysicalAddress))
            {
                return;
            }

            var ripPacket = Protocols.RIP.Parse(packet);
            if (ripPacket != null)
            {
                RIP(ripPacket);
                return;
            }

            var arpPacket = (ARPPacket)packet.Extract(typeof(ARPPacket));
            if (arpPacket != null)
            {
                ARP(arpPacket);
                return;
            }

            var ipPacket = (IPv4Packet)packet.Extract(typeof(IPv4Packet));
            if (ipPacket != null)
            {
                IP(ipPacket);
                return;
            }
        }

        public void RIP(RIPPacket RIPPacket)
        {
            Console.WriteLine("Got RIP.");
        }

        public void ARP(ARPPacket ARPPacket)
        {
            Console.WriteLine("Got ARP.");
        }

        public void IP(IPv4Packet IPv4Packet)
        {
            Console.WriteLine("Got IPV4.");
        }
    }
}
