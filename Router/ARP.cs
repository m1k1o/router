using PacketDotNet;
using Router.Helpers;
using SharpPcap;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace Router
{
    class ARP
    {
        static public TimeSpan Timeout = TimeSpan.FromMilliseconds(1500);
        static public TimeSpan Interval = TimeSpan.FromMilliseconds(500);

        static ManualResetEvent BlocingWaiting = new ManualResetEvent(false);

        public static bool ProxyEnabled { get; set; }

        static public void OnReceived(PhysicalAddress DestinationMac, ARPPacket ARPPacket, Interface Interface)
        {
            // Is packet valid response for me?
            if (
                Equals(DestinationMac, Interface.PhysicalAddress) && 
                Equals(ARPPacket.TargetHardwareAddress, Interface.PhysicalAddress) && 
                Equals(ARPPacket.TargetProtocolAddress, Interface.IPAddress)
            )
            {
                ARPTable.Instance.Push(ARPPacket.SenderProtocolAddress, ARPPacket.SenderHardwareAddress);

                // Blocking Notifyinng
                BlocingWaiting.Set();
                return;
            }

            // Is packet valid request?
            if (
                Equals(DestinationMac, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF")) &&
                (
                    Equals(ARPPacket.TargetHardwareAddress, PhysicalAddress.Parse("00-00-00-00-00-00")) ||
                    Equals(ARPPacket.TargetHardwareAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"))
                )
            )
            {
                // Am i asked?
                if (Equals(ARPPacket.TargetProtocolAddress, Interface.IPAddress))
                {
                    Protocols.ARP.SendResponse(ARPPacket.SenderHardwareAddress, ARPPacket.SenderProtocolAddress, Interface);
                    return;
                }

                // Is Porxy ARP turned on? And is request from outside of this interface?
                if (!ProxyEnabled || Interface.IsReachable(Interface.IPAddress))
                {
                    return;
                }

                // Exists this IP in RoutingTable?
                if (RoutingTable.Instance.Exists(ARPPacket.TargetProtocolAddress))
                {
                    Protocols.ARP.SendProxyResponse(ARPPacket.TargetProtocolAddress, ARPPacket.SenderHardwareAddress, ARPPacket.SenderProtocolAddress, Interface);
                }
            }
        }
        
        static public PhysicalAddress Lookup(IPAddress IPAddress, Interface Interface)
        {
            var PhysicalAddress = ARPTable.Instance.Find(IPAddress);
            if (PhysicalAddress != null)
            {
                return PhysicalAddress;
            }

            return Fetch(IPAddress, Interface);
        }

        static public PhysicalAddress Fetch(IPAddress IPAddress, Interface Interface)
        {
            BlocingWaiting.Reset();

            var timeoutDateTime = DateTime.Now + Timeout;
            while (DateTime.Now < timeoutDateTime)
            {
                // Send Request
                Protocols.ARP.SendRequest(IPAddress, Interface);

                // Wait for response
                if (BlocingWaiting.WaitOne(Interval))
                {
                    return ARPTable.Instance.Find(IPAddress);
                }
            }

            return null;
        }
    }
}
