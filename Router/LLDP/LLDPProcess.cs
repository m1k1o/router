using PacketDotNet.LLDP;
using System.Net.NetworkInformation;
using System.Threading;
using PacketDotNet;

namespace Router.LLDP
{
    static class LLDPProcess
    {
        public static PhysicalAddress LLDPMulticast = PhysicalAddress.Parse("01-80-C2-00-00-0E");
        public static ushort TimeToLive = 120;

        public static string SystemName = "WAN Router";
        public static string SystemDescription = "Lightweight C# Router";

        public static bool Running { get; private set; } = false;

        private static Thread Thread;
        private static int Sleep = 30000; // wait 30sec

        public static void Toggle()
        {
            if (Running)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private static void Start()
        {
            Running = true;

            Thread = new Thread(BackgroundThread);
            Thread.Start();
        }

        private static void Stop()
        {
            Running = false;
            if (!Thread.Join(500))
            {
                Thread.Abort();
            }
        }

        private static void BackgroundThread()
        {
            while (Running)
            {
                // Send to all
                foreach (var Interface in Interfaces.Instance.GetInteraces())
                {
                    try
                    {
                        Send(Interface);
                    }
                    catch { };
                }

                Thread.Sleep(Sleep);
            }
        }

        public static void Send(Interface Interface)
        {
            var TlvCollection = new TLVCollection
            {
                new ChassisID(ChassisSubTypes.NetworkAddress, Interface.IPAddress),
                new PortID(PortSubTypes.MACAddress, Interface.PhysicalAddress),
                new TimeToLive(TimeToLive),

                new PortDescription(Interface.FriendlyName),
                new SystemName(SystemName),
                new SystemDescription(SystemDescription),

                new SystemCapabilities((ushort)CapabilityOptions.Router, (ushort)CapabilityOptions.Router),

                new EndOfLLDPDU()
            };

            var LLDPPacket = new LLDPPacket
            {
                TlvCollection = TlvCollection
            };

            var EthernetPacket = new EthernetPacket(Interface.PhysicalAddress, LLDPMulticast, EthernetPacketType.LLDP)
            {
                PayloadPacket = new LLDPPacket(new PacketDotNet.Utils.ByteArraySegment(LLDPPacket.Bytes))
            };

            Interface.SendPacket(EthernetPacket.Bytes);
        }
    }
}
