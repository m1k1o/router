using PacketDotNet;
using Router.RIP;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Router
{
    class Interfaces
    {
        internal static Interfaces Instance { get; } = new Interfaces();

        private List<Interface> Available = new List<Interface>();

        private static object QueueLock = new object();
        private static List<Handler> HandlerQueue = new List<Handler>();

        private static bool PacketProcessingStop = false;
        private Thread PacketProcessingThread;

        public Interfaces()
        {
            // Print SharpPcap version
            Console.WriteLine("SharpPcap {0}.", SharpPcap.Version.VersionString);
            Console.WriteLine("Loading...");

            foreach (var Device in CaptureDeviceList.Instance)
            {
                Available.Add(new Interface(Device));
            }
        }
        
        public Interface GetInterfaceById(int ID)
        {
            return Available[ID];
        }

        public Interface GetInterfaceByName(string Name)
        {
            var i = 1;
            foreach (var Int in Available)
            {
                if (Int.Name == Name)
                {
                    return Int;
                }

                i++;
            }

            throw new Exception("No interface Found.");
        }

        public List<Interface> GetInteraces()
        {
            return Available;
        }

        internal static void OnCaptureStopped(object sender, CaptureStoppedEventStatus e)
        {
            Console.WriteLine("Capture stopped...");
        }

        internal static void OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var Handler = new Handler(e.Packet, Instance.GetInterfaceByName(e.Device.Name));

            // Unsupported packet
            if (!Handler.Exists())
            {
                return;
            }

            // Async processing
            if (Handler.CheckType(typeof(RIPPacket)) || Handler.CheckType(typeof(ARPPacket)))
            {
                Handler.Execute();
                return;
            }

            // Queued processing
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
                        e.Execute();
                    }
                }
            }
        }
    }
}
