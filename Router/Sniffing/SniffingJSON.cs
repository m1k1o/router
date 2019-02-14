using PacketDotNet;
using Router.Helpers;
using Router.LLDP;
using Router.Protocols;

namespace Router.Sniffing
{
    class SniffingJSON
    {
        public JSONObject Result { get; private set; } = new JSONObject();

        private Handler Handler;

        public SniffingJSON(Handler Handler)
        {
            this.Handler = Handler;
        }

        public void Extract()
        {
            Result.Push("layer", Handler.Layer);

            EthernetPacket(Handler.EthernetPacket);

            // ARPPacket
            if (Handler.EthernetPacket.Type == EthernetPacketType.Arp)
            {
                ARPPacket((ARPPacket)Handler.EthernetPacket.PayloadPacket);
                return;
            }

            // LLDPPacket
            if (Handler.EthernetPacket.Type == EthernetPacketType.LLDP)
            {
                LLDPPacket((LLDPPacket)Handler.EthernetPacket.PayloadPacket);
                return;
            }

            // IPv4Packet
            if (Handler.EthernetPacket.Type == EthernetPacketType.IpV4)
            {
                IPv4Packet(Handler.IPv4Packet);

                // TCPPacket
                if(Handler.IPv4Packet.Protocol == IPProtocolType.TCP)
                {
                    TCPPacket(Handler.TcpPacket);
                    return;
                }

                // UDPacket
                if (Handler.IPv4Packet.Protocol == IPProtocolType.UDP)
                {
                    UDPPacket(Handler.UdpPacket);

                    // RIPPacket
                    if (Handler.UdpPacket.SourcePort == 520)
                    {
                        RIPPacket(Protocols.RIP.Parse(Handler.UdpPacket));
                    }
                }
            }
        }

        private void EthernetPacket(EthernetPacket Packet)
        {
            var obj = new JSONObject();
            obj.Push("src_mac", Packet.SourceHwAddress);
            obj.Push("dst_mac", Packet.DestinationHwAddress);
            obj.Push("type", Packet.Type.ToString());

            Result.Push("eth", obj);
        }

        private void ARPPacket(ARPPacket Packet)
        {
            var obj = new JSONObject();
            obj.Push("op", Packet.Operation.ToString());
            obj.Push("sender_mac", Packet.SenderHardwareAddress);
            obj.Push("sender_ip", Packet.SenderProtocolAddress);
            obj.Push("target_mac", Packet.TargetHardwareAddress);
            obj.Push("target_ip", Packet.TargetProtocolAddress);

            Result.Push("arp", obj);
        }

        private void LLDPPacket(LLDPPacket Packet)
        {
            var obj = new JSONObject();

            // TODO: Bad practces
            var LLDPEntry = new LLDPEntry(Packet.TlvCollection, null);
            obj.Push("chassis_id", LLDPEntry.ChassisID.SubTypeValue);
            obj.Push("port_id", LLDPEntry.PortID.SubTypeValue);
            obj.Push("time_to_live", LLDPEntry.ExpiresIn);
            obj.Push("port_description", LLDPEntry.PortDescription == null ? null : LLDPEntry.PortDescription.StringValue);
            obj.Push("system_name", LLDPEntry.SystemName == null ? null : LLDPEntry.SystemName.StringValue);

            Result.Push("lldp", obj);
        }

        private void IPv4Packet(IPv4Packet Packet)
        {
            var obj = new JSONObject();
            obj.Push("src_ip", Packet.SourceAddress);
            obj.Push("dst_ip", Packet.DestinationAddress);
            obj.Push("ttl", Packet.TimeToLive);
            obj.Push("protocol", Packet.Protocol);

            Result.Push("ip", obj);
        }

        private void TCPPacket(TcpPacket Packet)
        {
            var obj = new JSONObject();
            obj.Push("src_port", Packet.SourcePort);
            obj.Push("dst_port", Packet.DestinationPort);

            Result.Push("udp", obj);
        }

        private void UDPPacket(UdpPacket Packet)
        {
            var obj = new JSONObject();
            obj.Push("src_port", Packet.SourcePort);
            obj.Push("dst_port", Packet.DestinationPort);

            Result.Push("udp", obj);
        }

        private void RIPPacket(RIPPacket Packet)
        {
            var obj = new JSONObject();
            obj.Push("cmd_type", Packet.CommandType.ToString());

            var arr = new JSONArray();
            var route = new JSONObject();
            foreach (var Route in Packet.RouteCollection)
            {
                route.Empty();

                route.Push("afi", Route.AddressFamilyIdentifier);
                route.Push("route_tag", Route.RouteTag);
                route.Push("ip", Route.IPAddress);
                route.Push("mask", Route.IPSubnetMask);
                route.Push("network", Route.IPNetwork);
                route.Push("next_hop", Route.NextHop);
                route.Push("metric", Route.Metric);

                arr.Push(route);
            }

            obj.Push("routes", arr);

            Result.Push("rip", obj);
        }
    }
}
