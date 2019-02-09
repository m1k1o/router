using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Router.RIP
{
    static class RIPInterfaces
    {
        public static TimeSpan UpdateTimer { get; set; } = TimeSpan.FromSeconds(30);

        private static List<Interface> Active { get; } = new List<Interface>();

        private static List<Interface> Running { get; } = new List<Interface>();

        private static ManualResetEvent StopRequest = new ManualResetEvent(false);
        private static Thread Thread;

        public static bool IsRunning(Interface Interface)
        {
            return Running.Exists(Entry => Equals(Entry, Interface));
        }

        public static bool IsActive(Interface Interface)
        {
            return Active.Exists(Entry => Equals(Entry, Interface));
        }

        public static void Add(Interface Interface)
        {
            Active.Add(Interface);

            if (Interface.Running)
            {
                Start(Interface);
            }

            Interface.OnStarted += Start;
            Interface.OnStopped += Stop;
        }

        public static void Remove(Interface Interface)
        {
            Active.Remove(Interface);

            Interface.OnStarted -= Start;
            Interface.OnStopped -= Stop;

            if (Interface.Running)
            {
                Stop(Interface);
            }
        }

        private static void Start(Interface Interface)
        {
            var RIPEntry = new RIPEntry(Interface, Interface.IPNetwork, null, 1)
            {
                SyncWithRT = false,
                CanBeUpdated = false,
                TimersEnabled = false
            };

            Running.Add(Interface);
            RIPTable.Instance.Add(RIPEntry);

            RIPResponse.SendTriggeredUpdate(Interface, RIPEntry);

            RIPRequest.AskForUpdate(Interface);
        }

        private static void Stop(Interface Interface)
        {
            var RIPEntry = RIPTable.Instance.Find(Interface, Interface.IPNetwork);
            if (RIPEntry == null)
            {
                throw new Exception("RIPEntry not found while Stopping");
            }

            RIPEntry.PossibblyDown = true;

            RIPResponse.SendTriggeredUpdate(Interface, RIPEntry);

            Running.Remove(Interface);
            RIPTable.Instance.Remove(Interface);

            RIPTable.Instance.SyncWithRT();
        }

        public static List<Interface> GetRunningInterfaces()
        {
            return Running.ToList();
        }

        public static List<Interface> GetActiveInterfaces()
        {
            return Active.ToList();
        }

        public static void StartUpdates()
        {
            StopRequest.Reset();

            Thread = new Thread(SendUpdates);
            Thread.Start();
        }

        public static void StopUpdates()
        {
            StopRequest.Set();
            Thread.Join();
        }

        public static void SendUpdates()
        {
            do
            {
                RIPResponse.SendUpdate();

                if (StopRequest.WaitOne(UpdateTimer))
                {
                    Console.WriteLine("Terminating thread...");
                    break;
                }

                RIPTable.Instance.GarbageCollector();
            }
            while (true);
        }
    }
}
