using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Router.RIP
{
    static class RIPInterfaces
    {
        static public TimeSpan UpdateTimer = TimeSpan.FromSeconds(30);

        static List<Interface> Available { get; } = new List<Interface>();
        static List<Interface> Active { get; } = new List<Interface>();

        static ManualResetEvent StopRequest = new ManualResetEvent(false);
        static Thread Thread;

        static public bool IsActive(Interface Interface)
        {
            return Active.Exists(Entry => Entry == Interface);
        }

        static public void Add(Interface Interface)
        {
            Available.Add(Interface);
            Interface.RegisterOnStopped(Stop);
        }

        static public void Start(Interface Interface)
        {
            if (!Interface.Running)
            {
                throw new Exception("Interface must be running.");
            }

            var RIPEntry = new RIPEntry(Interface, Interface.IPNetwork, null, 1);
            RIPEntry.SyncWithRT = false;
            RIPEntry.AllowUpdates = false;

            Active.Add(Interface);
            RIPTable.Instance.Add(RIPEntry);

            RIPResponse.SendTriggeredUpdate(Interface, RIPEntry);
        }

        static public void Remove(Interface Interface)
        {
            Available.Remove(Interface);
            Interface.UnregisterOnStopped(Stop);
            Stop(Interface);
        }

        static public void Stop(Interface Interface)
        {
            if (!IsActive(Interface))
            {
                return;
            }

            var RIPEntry = RIPTable.Instance.Find(Interface, Interface.IPNetwork);
            RIPEntry.PossibblyDown = true;

            RIPResponse.SendTriggeredUpdate(Interface, RIPEntry);

            Active.Remove(Interface);
            RIPTable.Instance.Remove(Interface);

            RIPTable.Instance.SyncWithRT();
        }

        static public List<Interface> GetActiveInterfaces()
        {
            return Active.ToList();
        }

        static public List<Interface> GetAvailableInterfaces()
        {
            return Available.ToList();
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
                var Interfaces = GetActiveInterfaces();
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
