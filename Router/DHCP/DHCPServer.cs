using Router.Protocols;
using Router.Protocols.DHCPOptions;
using System;
using System.Net;

namespace Router.DHCP
{
    class DHCPServer
    {
        public static void OnReceived(IPAddress DestinationIP, DHCPPacket DHCPPacket, Interface Interface)
        {
            var Options = DHCPPacket.Options;

            if (DHCPPacket.OperationCode != DHCPOperatonCode.BootRequest)
            {
                Console.WriteLine("Only DHCPOperatonCode BootRequest accepting.");
                return;
            }

            if (DHCPPacket.Options.MessageType == DHCPMessageType.Discover)
            {
                Offer(DHCPPacket, Interface);
                return;
            }

            // Check if is for me
            IPAddress DHCPServerID = DestinationIP;
            foreach (var Option in Options)
            {
                if (Option is DHCPServerIdentifierOption)
                {
                    DHCPServerID = ((DHCPServerIdentifierOption)Option).IPAddress;
                    break;
                }
            }

            if (!Equals(DHCPServerID, Interface.IPAddress))
            {
                Console.WriteLine("Invalid DHCPServerIdentifierOption.");
                return;
            }

            if (DHCPPacket.Options.MessageType == DHCPMessageType.Request)
            {
                Lease(DHCPPacket, Interface);
                return;
            }

            if (DHCPPacket.Options.MessageType == DHCPMessageType.Release)
            {
                Release(DHCPPacket, Interface);
                return;
            }
        }

        public static void Offer(DHCPPacket DHCPPacket, Interface Interface)
        {
            var TransactionID = DHCPPacket.TransactionID;
            var ClientMACAddress = DHCPPacket.ClientMACAddress;

            var DHCPLease = DHCPTable.Instance.Find(ClientMACAddress, Interface);
            if (DHCPLease != null)
            {
                DHCPLease.IsOffered = true;
                Protocols.DHCP.SendOffer(TransactionID, ClientMACAddress, DHCPLease.IPAddress, Interface);
                return;
            }

            // Create new Lease from Pool
            Console.WriteLine("Create new Lease from Pool: NotImplementedException");
            //throw new NotImplementedException();
        }

        public static void Lease(DHCPPacket DHCPPacket, Interface Interface)
        {
            var TransactionID = DHCPPacket.TransactionID;
            var ClientMACAddress = DHCPPacket.ClientMACAddress;

            var DHCPLease = DHCPTable.Instance.Find(ClientMACAddress, Interface);
            if (DHCPLease != null)
            {
                DHCPLease.IsOffered = false;
                DHCPLease.IsLeased = true;

                Protocols.DHCP.SendACK(TransactionID, ClientMACAddress, DHCPLease.IPAddress, Interface);
                return;
            }
        }

        public static void Release(DHCPPacket DHCPPacket, Interface Interface)
        {
            // Remove from Lease
            throw new NotImplementedException();
        }
    }
}
