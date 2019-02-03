﻿using PacketDotNet;
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

        public bool Running { get; private set; } = false;

        public Interfaces()
        {
            // Print SharpPcap version
            Console.WriteLine("SharpPcap {0}.", SharpPcap.Version.VersionString);

            foreach (var Device in CaptureDeviceList.Instance)
            {
                Available.Add(new Interface(Device));
            }
        }

        public void SelectInterface(int ID)
        {
            GetInterfaceById(ID).Selected = true;
        }

        public void UnselectInterface(int ID)
        {
            GetInterfaceById(ID).Selected = false;
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

        public void Open()
        {
            var Interfaces = GetInteraces();

            foreach (var Interface in Interfaces)
            {
                if (!Interface.Selected)
                {
                    continue;
                }

                Interface.Device.Open(DeviceMode.Promiscuous, 1);
                Interface.Device.OnPacketArrival += new PacketArrivalEventHandler(OnPacketArrival);
                Interface.Device.OnCaptureStopped += new CaptureStoppedEventHandler(OnCaptureStopped);
                Interface.Device.StartCapture();
            }

            // start the background thread
            PacketProcessingThread = new Thread(BackgroundThread);
            PacketProcessingThread.Start();
            PacketProcessingStop = false;

            Running = true;
        }

        public void Close()
        {
            // ask the background thread to shut down
            PacketProcessingStop = true;

            // wait for the background thread to terminate
            PacketProcessingThread.Join();

            var Interfaces = GetInteraces();

            foreach (var Interface in Interfaces)
            {
                if (!Interface.Selected)
                {
                    continue;
                }

                try
                {
                    Interface.Device.StopCapture();
                }
                catch { };

                try
                {

                    Interface.Device.Close();
                }
                catch { };
            }

            Running = false;
        }

        private static void OnCaptureStopped(object sender, CaptureStoppedEventStatus e)
        {
            Console.WriteLine("Capture stopped...");
        }

        private static void OnPacketArrival(object sender, CaptureEventArgs e)
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
