using Router.Protocols;
using Router.Protocols.DHCPOptions;
using System;
using System.Net;

namespace Router.DHCP
{
    static class DHCPServer
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

            // Get Pool
            if (!DHCPPool.Interfaces.ContainsKey(Interface))
            {
                Console.WriteLine("No DHCP Pool defined for " + Interface);
                return;
            }
            var Pool = DHCPPool.Interfaces[Interface];

            // Fetch unused IP
            IPAddress ClientIP;
            do
            {
                ClientIP = Pool.Allocate();
            }
            while (DHCPTable.Instance.Exists(ClientIP));

            if (ClientIP == null)
            {
                Console.WriteLine("No available IP in DHCP Pool for " + Interface);
                return;
            }

            // Add new lease
            DHCPLease = new DHCPLease(ClientMACAddress, Interface, ClientIP)
            {
                IsDynamic = true,
                IsOffered = true
            };
            DHCPTable.Instance.Push(DHCPLease);
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
            var ClientMACAddress = DHCPPacket.ClientMACAddress;

            var DHCPLease = DHCPTable.Instance.Find(ClientMACAddress, Interface);
            if (DHCPLease == null)
            {
                return;
            }

            DHCPLease.IsOffered = false;
            DHCPLease.IsLeased = false;

            if (DHCPLease.IsDynamic)
            {
                // Remove from Table
                DHCPTable.Instance.Remove(DHCPLease);
            }
        }
    }
}
