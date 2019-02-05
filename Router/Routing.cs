using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Router
{
    class Routing
    {
        static public void OnReceived(IPv4Packet IPPacket, Interface Interface)
        {
            IPPacket.TimeToLive--;
            if(IPPacket.TimeToLive <= 0)
            {
                Console.WriteLine("TTL <= 0; drop packet.");
                return;
            }

            var RoutingEntry = RoutingTable.Instance.Lookup(IPPacket.DestinationAddress);
            if (RoutingEntry == null)
            {
                Console.WriteLine("No RoutingEntry for {0}.", IPPacket.DestinationAddress);
                return;
            }

            if (RoutingEntry.Interface == null)
            {
                Console.WriteLine("No Interface after RoutingTable Lookup for {0}.", IPPacket.DestinationAddress);
                return;
            }

            IPAddress ARPRequestIP;
            
            // Next Hop IP
            if (RoutingEntry.NextHopIP != null)
            {
                ARPRequestIP = RoutingEntry.NextHopIP;
            } else
            {
                ARPRequestIP = IPPacket.DestinationAddress;
            }

            PhysicalAddress DestionationMac = ARP.Lookup(ARPRequestIP, Interface);
            if (DestionationMac == null)
            {
                Console.WriteLine("No DestionationMac after ARP Lookup for {0}.", ARPRequestIP);
                return;
            }

            // Send
            var ethernetPacket = new EthernetPacket(Interface.PhysicalAddress, DestionationMac, EthernetPacketType.IpV4);
            ethernetPacket.PayloadData = IPPacket.Bytes;
            Interface.SendPacket(ethernetPacket.Bytes);
        }
    }
}
