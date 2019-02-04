using PacketDotNet;
using SharpPcap;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace Router
{
    class ARP
    {
        static private TimeSpan Timeout = new TimeSpan(0, 0, 3);
        static private TimeSpan Interval = new TimeSpan(0, 0, 1);

        static ManualResetEvent BlocingWaiting = new ManualResetEvent(false);

        public static bool ProxyARP {
            get => false;
            set
            {
                throw new NotImplementedException();
            }
        }

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
                Equals(ARPPacket.TargetHardwareAddress, PhysicalAddress.Parse("00-00-00-00-00-00"))
            )
            {
                // Am i asked?
                if(Equals(ARPPacket.TargetProtocolAddress, Interface.IPAddress))
                {
                    Protocols.ARP.SendResponse(ARPPacket.SenderHardwareAddress, ARPPacket.SenderProtocolAddress, Interface);
                    return;
                }

                // Porxy ARP
                if (!ProxyARP)
                {
                    return;
                }

                // Lookup in Routing table
                throw new NotImplementedException();

                //if(/* in routing table */)
                //{
                //    Protocols.ARP.SendProxyResponse(ARPPacket.TargetProtocolAddress, ARPPacket.SenderHardwareAddress, ARPPacket.SenderProtocolAddress, Interface);
                //}
            }
        }
        
        static public PhysicalAddress Lookup(IPAddress IPAddress, Interface Interface)
        {
            var PhysicalAddress = ARPTable.Instance.Find(IPAddress);
            if(PhysicalAddress != null)
            {
                return PhysicalAddress;
            }

            PhysicalAddress = Fetch(IPAddress, Interface);
            if (PhysicalAddress == null)
            {
                return null;
            }

            ARPTable.Instance.Push(IPAddress, PhysicalAddress);
            return PhysicalAddress;
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
