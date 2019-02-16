using PacketDotNet;
using System;
using System.Net.NetworkInformation;
using Router.ARP;
using System.Threading;
using System.Collections.Generic;

namespace Router
{
    static class Routing
    {
        private static object QueueLock = new object();
        private static List<Handler> HandlerQueue = new List<Handler>();

        private static bool PacketProcessingStop = false;
        private static Thread PacketProcessingThread;

        public static void Start()
        {
            PacketProcessingThread = new Thread(BackgroundThread);
            PacketProcessingThread.Start();
            PacketProcessingStop = false;
        }

        public static void Stop()
        {
            PacketProcessingStop = true;
            PacketProcessingThread.Join();
        }

        public static void AddToQueue(Handler Handler)
        {
            lock (QueueLock)
            {
                HandlerQueue.Add(Handler);
            }
        }

        private static void BackgroundThread()
        {
            while (!PacketProcessingStop)
            {
                bool shouldSleep = true;

                lock (QueueLock)
                {
                    if (HandlerQueue.Count != 0)
                    {
                        shouldSleep = false;
                    }
                }

                if (shouldSleep)
                {
                    Thread.Sleep(250);
                }
                else // should process the queue
                {
                    List<Handler> ourQueue;
                    lock (QueueLock)
                    {
                        // swap queues, giving the capture callback a new one
                        ourQueue = HandlerQueue;
                        HandlerQueue = new List<Handler>();
                    }

                    foreach (var e in ourQueue)
                    {
                        PerformRouting(e.IPv4Packet, e.Interface);
                    }
                }
            }
        }

        private static void PerformRouting(IPv4Packet IPPacket, Interface Interface)
        {
            IPPacket.TimeToLive--;
            IPPacket.Checksum = IPPacket.CalculateIPChecksum();

            if (IPPacket.TimeToLive <= 0)
            {
                Console.WriteLine("TimeToLive reached 0, dropping packet.");
                return;
            }

            var RoutingEntry = RoutingTable.Instance.Lookup(IPPacket.DestinationAddress);
            if (RoutingEntry == null)
            {
                Console.WriteLine("No RoutingEntry for {0}.", IPPacket.DestinationAddress);
                return;
            }

            if (!RoutingEntry.HasInterface)
            {
                Console.WriteLine("No Interface after RoutingTable Lookup for {0}.", IPPacket.DestinationAddress);
                return;
            }

            if (Equals(RoutingEntry.Interface, Interface))
            {
                Console.WriteLine("Out and In Interfaces match for {0}.", IPPacket.DestinationAddress);
                return;
            }

            PhysicalAddress DestionationMac = ARPMiddleware.Lookup(RoutingEntry.NextHopIP, RoutingEntry.Interface);
            if (DestionationMac == null)
            {
                Console.WriteLine("No DestionationMac after ARP Lookup for {0}.", RoutingEntry.NextHopIP);
                return;
            }

            var ethernetPacket = new EthernetPacket(RoutingEntry.Interface.PhysicalAddress, DestionationMac, EthernetPacketType.IpV4)
            {
                PayloadData = IPPacket.Bytes
            };

            // Send
            RoutingEntry.Interface.SendPacket(ethernetPacket.Bytes);
        }
    }
}
