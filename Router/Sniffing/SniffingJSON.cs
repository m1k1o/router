using PacketDotNet;
using Router.Helpers;
using Router.LLDP;
using Router.Protocols;

namespace Router.Sniffing
{
    class SniffingJSON
    {
        /*
        public Old_JSONObject Result { get; private set; } = new Old_JSONObject();

        private Handler Handler;

        public SniffingJSON(Handler Handler)
        {
            this.Handler = Handler;
        }

        public void Extract()
        {
            Result.Add("layer", Handler.Layer);

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
                        return;
                    }

                    // DHCPPacket
                    if (Handler.UdpPacket.SourcePort == 67 || Handler.UdpPacket.SourcePort == 68)
                    {
                        DHCPPacket(new DHCPPacket(Handler.UdpPacket.PayloadData));
                        return;
                    }
                }
            }
        }

        private void EthernetPacket(EthernetPacket Packet)
        {
            var obj = new Old_JSONObject();
            obj.Add("src_mac", Packet.SourceHwAddress);
            obj.Add("dst_mac", Packet.DestinationHwAddress);
            obj.Add("type", Packet.Type.ToString());

            Result.Add("eth", obj);
        }

        private void ARPPacket(ARPPacket Packet)
        {
            var obj = new Old_JSONObject();
            obj.Add("op", Packet.Operation.ToString());
            obj.Add("sender_mac", Packet.SenderHardwareAddress);
            obj.Add("sender_ip", Packet.SenderProtocolAddress);
            obj.Add("target_mac", Packet.TargetHardwareAddress);
            obj.Add("target_ip", Packet.TargetProtocolAddress);

            Result.Add("arp", obj);
        }

        private void LLDPPacket(LLDPPacket Packet)
        {
            var obj = new Old_JSONObject();

            // TODO: Bad practces
            var LLDPEntry = new LLDPEntry(Packet.TlvCollection, null);
            obj.Add("chassis_id", LLDPEntry.ChassisID.SubTypeValue);
            obj.Add("port_id", LLDPEntry.PortID.SubTypeValue);
            obj.Add("time_to_live", LLDPEntry.ExpiresIn);
            obj.Add("port_description", LLDPEntry.PortDescription == null ? null : LLDPEntry.PortDescription.StringValue);
            obj.Add("system_name", LLDPEntry.SystemName == null ? null : LLDPEntry.SystemName.StringValue);

            Result.Add("lldp", obj);
        }

        private void IPv4Packet(IPv4Packet Packet)
        {
            var obj = new Old_JSONObject();
            obj.Add("src_ip", Packet.SourceAddress);
            obj.Add("dst_ip", Packet.DestinationAddress);
            obj.Add("ttl", Packet.TimeToLive);
            obj.Add("protocol", Packet.Protocol);

            Result.Add("ip", obj);
        }

        private void TCPPacket(TcpPacket Packet)
        {
            var obj = new Old_JSONObject();
            obj.Add("src_port", Packet.SourcePort);
            obj.Add("dst_port", Packet.DestinationPort);

            Result.Add("udp", obj);
        }

        private void UDPPacket(UdpPacket Packet)
        {
            var obj = new Old_JSONObject();
            obj.Add("src_port", Packet.SourcePort);
            obj.Add("dst_port", Packet.DestinationPort);

            Result.Add("udp", obj);
        }

        private void RIPPacket(RIPPacket Packet)
        {
            var obj = new Old_JSONObject();
            obj.Add("cmd_type", Packet.CommandType.ToString());

            var arr = new Old_JSONArray();
            var route = new Old_JSONObject();
            foreach (var Route in Packet.RouteCollection)
            {
                route.Empty();

                route.Add("afi", Route.AddressFamilyIdentifier);
                route.Add("route_tag", Route.RouteTag);
                route.Add("ip", Route.IPAddress);
                route.Add("mask", Route.IPSubnetMask);
                route.Add("network", Route.IPNetwork);
                route.Add("next_hop", Route.NextHop);
                route.Add("metric", Route.Metric);

                arr.Push(route);
            }

            obj.Add("routes", arr);

            Result.Add("rip", obj);
        }

        private void DHCPPacket(DHCPPacket DHCPPacket)
        {
            var obj = new Old_JSONObject();
            obj.Add("operation_code", DHCPPacket.OperationCode);
            obj.Add("transaction_id", DHCPPacket.TransactionID);

            obj.Add("client_ip", DHCPPacket.YourClientIPAddress);
            obj.Add("server_ip", DHCPPacket.NextServerIPAddress);
            obj.Add("client_mac", DHCPPacket.ClientMACAddress);

            var Options = DHCPPacket.Options;
            obj.Add("message_type", Options.MessageType);

            Result.Add("dhcp", obj);
        }
        */
    }
}
