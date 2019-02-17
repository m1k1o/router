using PacketDotNet;
using Router.Helpers;
using Router.Protocols.DHCPOptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Protocols
{
    static class DHCP
    {
        public static TimeSpan IPAddressLeaseTime { get; set; } = TimeSpan.Parse("01:00:00");
        public static TimeSpan RenewalTimeValue = TimeSpan.Parse("00:30:00");
        public static TimeSpan RebindingTimeValue = TimeSpan.Parse("00:52:30");
        public static List<IPAddress> DNS = new List<IPAddress>()
        {
            // Cloudflare Public DNS
            IPAddress.Parse("1.1.1.1"),
            IPAddress.Parse("1.0.0.1"),
            
            // Google Public DNS
            IPAddress.Parse("8.8.8.8"),
            IPAddress.Parse("8.8.4.4")
        };

        public static void SendDiscover(uint TransactionID, Interface Interface)
        {
            var Options = new DHCPOptionCollection
            {
                new DHCPMessageTypeOption(DHCPMessageType.Discover),
                new DHCPClientIdentifierOption(Interface.PhysicalAddress),
                new DHCPRequestedIPAddressOption(IPAddress.Parse("0.0.0.0")),
                new DHCPParameterRequestListOption(new List<DHCPOptionCode> {
                    DHCPOptionCode.SubnetMask,
                    DHCPOptionCode.Router,
                    DHCPOptionCode.DomainNameServer
                }),
                new DHCPEndOption()
            };

            var dhcpPacket = new DHCPPacket(DHCPOperatonCode.BootRequest, TransactionID, Options)
            {
                ClientMACAddress = Interface.PhysicalAddress
            };

            var udpPacket = new UdpPacket(68, 67)
            {
                PayloadData = dhcpPacket.Bytes
            };

            var ipPacket = new IPv4Packet(IPAddress.Parse("0.0.0.0"), IPAddress.Parse("255.255.255.255"))
            {
                PayloadPacket = udpPacket
            };
            ipPacket.Checksum = ipPacket.CalculateIPChecksum();

            var ethernetPacket = new EthernetPacket(Interface.PhysicalAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetPacketType.IpV4)
            {
                PayloadPacket = ipPacket
            };

            Interface.SendPacket(ethernetPacket.Bytes);
        }
        
        public static void SendOffer(uint TransactionID, PhysicalAddress ClientMAC, IPAddress ClientIPAddress, Interface Interface)
        {
            var Options = new DHCPOptionCollection
            {
                new DHCPMessageTypeOption(DHCPMessageType.Offer),
                new DHCPSubnetMaskOption(Interface.IPNetwork.SubnetMask),
                new DHCPRouterOption(new List<IPAddress> {
                    Interface.IPAddress
                }),
                new DHCPIPAddressLeaseTimeOption(IPAddressLeaseTime),
                new DHCPRenewalTimeValueOption(RenewalTimeValue),
                new DHCPRebindingTimeValueOption(RebindingTimeValue),
                new DHCPServerIdentifierOption(Interface.IPAddress),
                new DHCPDomainNameServerOption(DNS),
                new DHCPEndOption()
            };

            var dhcpPacket = new DHCPPacket(DHCPOperatonCode.BootReply, TransactionID, Options)
            {
                ClientMACAddress = ClientMAC,
                YourClientIPAddress = ClientIPAddress,
                NextServerIPAddress = Interface.IPAddress
            };

            var udpPacket = new UdpPacket(67, 68)
            {
                PayloadData = dhcpPacket.Bytes
            };

            var ipPacket = new IPv4Packet(Interface.IPAddress, IPAddress.Parse("255.255.255.255"))
            {
                PayloadPacket = udpPacket
            };
            ipPacket.Checksum = ipPacket.CalculateIPChecksum();

            var ethernetPacket = new EthernetPacket(Interface.PhysicalAddress, ClientMAC, EthernetPacketType.IpV4)
            {
                PayloadPacket = ipPacket
            };

            Interface.SendPacket(ethernetPacket.Bytes);
        }

        public static void SendRequest(uint TransactionID, IPAddress RequestedIPAddress, IPAddress DHCPIPAddress, Interface Interface)
        {
            var Options = new DHCPOptionCollection
            {
                new DHCPMessageTypeOption(DHCPMessageType.Request),
                new DHCPClientIdentifierOption(Interface.PhysicalAddress),
                new DHCPRequestedIPAddressOption(RequestedIPAddress),
                new DHCPServerIdentifierOption(DHCPIPAddress),
                new DHCPParameterRequestListOption(new List<DHCPOptionCode> {
                    DHCPOptionCode.SubnetMask,
                    DHCPOptionCode.Router,
                    DHCPOptionCode.DomainNameServer
                }),
                new DHCPEndOption()
            };

            var dhcpPacket = new DHCPPacket(DHCPOperatonCode.BootRequest, TransactionID, Options)
            {
                ClientMACAddress = Interface.PhysicalAddress
            };

            var udpPacket = new UdpPacket(68, 67)
            {
                PayloadData = dhcpPacket.Bytes
            };

            var ipPacket = new IPv4Packet(IPAddress.Parse("0.0.0.0"), IPAddress.Parse("255.255.255.255"))
            {
                PayloadPacket = udpPacket
            };
            ipPacket.Checksum = ipPacket.CalculateIPChecksum();

            var ethernetPacket = new EthernetPacket(Interface.PhysicalAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetPacketType.IpV4)
            {
                PayloadPacket = ipPacket
            };

            Interface.SendPacket(ethernetPacket.Bytes);
        }

        public static void SendACK(uint TransactionID, PhysicalAddress ClientMAC, IPAddress ClientIPAddress, Interface Interface)
        {
            var Options = new DHCPOptionCollection
            {
                new DHCPMessageTypeOption(DHCPMessageType.Ack),
                new DHCPSubnetMaskOption(Interface.IPNetwork.SubnetMask),
                new DHCPRouterOption(new List<IPAddress> {
                    Interface.IPAddress
                }),
                new DHCPIPAddressLeaseTimeOption(IPAddressLeaseTime),
                new DHCPRenewalTimeValueOption(RenewalTimeValue),
                new DHCPRebindingTimeValueOption(RebindingTimeValue),
                new DHCPServerIdentifierOption(Interface.IPAddress),
                new DHCPDomainNameServerOption(DNS),
                new DHCPEndOption()
            };

            var dhcpPacket = new DHCPPacket(DHCPOperatonCode.BootReply, TransactionID, Options)
            {
                ClientMACAddress = ClientMAC,
                YourClientIPAddress = ClientIPAddress,
                NextServerIPAddress = Interface.IPAddress
            };

            var udpPacket = new UdpPacket(67, 68)
            {
                PayloadData = dhcpPacket.Bytes
            };

            var ipPacket = new IPv4Packet(Interface.IPAddress, IPAddress.Parse("255.255.255.255"))
            {
                PayloadPacket = udpPacket
            };
            ipPacket.Checksum = ipPacket.CalculateIPChecksum();

            var ethernetPacket = new EthernetPacket(Interface.PhysicalAddress, ClientMAC, EthernetPacketType.IpV4)
            {
                PayloadPacket = ipPacket
            };

            Interface.SendPacket(ethernetPacket.Bytes);
        }

        public static void Test() {
            var Interface = Interfaces.Instance.GetInterfaceById("2");
            Interface.SetIP(IPAddress.Parse("192.168.1.5"), IPSubnetMask.Parse("255.255.255.0"));
            Interface.Start();

            SendDiscover(10, Interface);
        }
    }
}
