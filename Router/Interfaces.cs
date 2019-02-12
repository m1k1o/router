using PacketDotNet;
using Router.Protocols;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Router
{
    class Interfaces
    {
        public static Interfaces Instance { get; } = new Interfaces();

        private List<Interface> Available = new List<Interface>();
        /*
        private static object QueueLock = new object();
        private static List<Handler> HandlerQueue = new List<Handler>();

        private static bool PacketProcessingStop = false;
        private Thread PacketProcessingThread;
        */
        private Interfaces()
        {
            // Print SharpPcap version
            Console.WriteLine("SharpPcap {0}.", SharpPcap.Version.VersionString);
            Console.WriteLine("Loading...");

            var i = 0;
            foreach (var Device in CaptureDeviceList.Instance)
            {
                Available.Add(new Interface(Device, i++));
            }

            //StartProcessing();
        }
        /*
        public void StartProcessing()
        {
            // start the background thread
            PacketProcessingThread = new Thread(BackgroundThread);
            PacketProcessingThread.Start();
            PacketProcessingStop = false;
        }

        public void StopProcessing()
        {
            // ask the background thread to shut down
            PacketProcessingStop = true;

            // wait for the background thread to terminate
            PacketProcessingThread.Join();
        }
        */
        public Interface GetInterfaceById(string ID)
        {
            return Available[Int32.Parse(ID)];
        }

        public Interface GetInterfaceById(int ID)
        {
            return Available[ID];
        }

        public Interface GetInterfaceByName(string Name)
        {
            return Available.Find(Interface => Interface.Name == Name);
        }

        public List<Interface> GetInteraces()
        {
            return Available.ToList();
        }
        /*
        public static void OnPacketArrival(RawCapture Packet, Interface Interface)
        {
            var Handler = new Handler(Packet, Interface);

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
        */
    }
}
