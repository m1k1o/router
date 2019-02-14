using PacketDotNet;
using Router.Helpers;
using Router.Protocols.DHCPOptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Router.Protocols
{
    class DHCP
    {
        public static void Test() {
            var Interface = Interfaces.Instance.GetInterfaceById("2");
            Interface.SetIP(IPAddress.Parse("192.168.1.5"), IPSubnetMask.Parse("255.255.255.0"));
            Interface.Start();

            Console.WriteLine("Started");

            var Options = new DHCPOptionCollection
            {
                new DHCPPadOption(5),
                new DHCPSubnetMaskOption(IPAddress.Parse("192.168.2.5")),
                new DHCPRouterOption(new List<IPAddress> {
                    IPAddress.Parse("8.8.8.8"),
                    IPAddress.Parse("10.10.2.5"),
                    IPAddress.Parse("192.168.1.0")
                }),
                new DHCPDomainNameServerOption(new List<IPAddress> {
                    IPAddress.Parse("8.9.8.8"),
                    IPAddress.Parse("10.1.2.5"),
                    IPAddress.Parse("192.4.1.0")
                }),
                new DHCPRequestedIPAddressOption(IPAddress.Parse("9.8.1.0")),
                new DHCPIPAddressLeaseTimeOption(569),
                new DHCPMessageTypeOption(DHCPMessageType.Ack),
                new DHCPServerIdentifierOption(IPAddress.Parse("1.2.3.4")),
                new DHCPParameterRequestListOption(new List<DHCPOptionCode> {
                    DHCPOptionCode.IPAddressLeaseTime,
                    DHCPOptionCode.RequestedIPAddress,
                    DHCPOptionCode.Router
                }),
                new DHCPEndOption()
            };

            var dhcpPacket = new DHCPPacket
            {
                OperationCode = DHCPOperatonCode.BootRequest,
                HardwareType = LinkLayers.Ethernet,
                HardwareAddressLength = 6,
                TransactionID = 0x3903F326,
                ClientMACAddress = Interface.PhysicalAddress,
                IsDHCP = true,
                Options = Options
            };

            Console.WriteLine(Encoding.Default.GetString(dhcpPacket.Options.Bytes));

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
            Console.WriteLine("Sent");
        }
    }
}
