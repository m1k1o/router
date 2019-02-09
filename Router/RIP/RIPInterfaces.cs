using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Router.RIP
{
    static class RIPInterfaces
    {
        static public TimeSpan UpdateTimer { get; set; } = TimeSpan.FromSeconds(30);

        static private List<Interface> Active { get; } = new List<Interface>();

        static private List<Interface> Running { get; } = new List<Interface>();

        static private ManualResetEvent StopRequest = new ManualResetEvent(false);
        static private Thread Thread;

        static public bool IsRunning(Interface Interface)
        {
            return Running.Exists(Entry => Equals(Entry, Interface));
        }

        static public bool IsActive(Interface Interface)
        {
            return Active.Exists(Entry => Equals(Entry, Interface));
        }

        static public void Add(Interface Interface)
        {
            Active.Add(Interface);

            if (Interface.Running)
            {
                Start(Interface);
            }

            Interface.OnStarted += Start;
            Interface.OnStopped += Stop;
        }

        static public void Remove(Interface Interface)
        {
            Active.Remove(Interface);

            Interface.OnStarted -= Start;
            Interface.OnStopped -= Stop;

            if (Interface.Running)
            {
                Stop(Interface);
            }
        }

        static private void Start(Interface Interface)
        {
            var RIPEntry = new RIPEntry(Interface, Interface.IPNetwork, null, 1)
            {
                SyncWithRT = false,
                AllowUpdates = false
            };

            Running.Add(Interface);
            RIPTable.Instance.Add(RIPEntry);

            RIPResponse.SendTriggeredUpdate(Interface, RIPEntry);
        }

        static private void Stop(Interface Interface)
        {
            var RIPEntry = RIPTable.Instance.Find(Interface, Interface.IPNetwork);
            RIPEntry.PossibblyDown = true;

            RIPResponse.SendTriggeredUpdate(Interface, RIPEntry);

            Running.Remove(Interface);
            RIPTable.Instance.Remove(Interface);

            RIPTable.Instance.SyncWithRT();
        }

        static public List<Interface> GetRunningInterfaces()
        {
            return Running.ToList();
        }

        static public List<Interface> GetActiveInterfaces()
        {
            return Active.ToList();
        }

        static public void StartUpdates()
        {
            StopRequest.Reset();

            Thread = new Thread(SendUpdates);
            Thread.Start();
        }

        static public void StopUpdates()
        {
            StopRequest.Set();
            Thread.Join();
        }

        static public void SendUpdates()
        {
            do
            {
                var Interfaces = GetRunningInterfaces();
                foreach (var Interface in Interfaces)
                {
                    var RIPResponse = new RIPResponse(Interface);
                    RIPResponse.Send();
                }

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
