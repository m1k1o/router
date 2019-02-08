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

        static List<Interface> Interfaces { get; } = new List<Interface>();

        static ManualResetEvent StopRequest = new ManualResetEvent(false);
        static Thread Thread;

        static public void Start()
        {
            StopRequest.Reset();

            Thread = new Thread(SendUnsolicitedUpdates);
            Thread.Start();
        }

        static public void Stop()
        {
            StopRequest.Set();
            Thread.Join();
        }

        static public void Add(Interface Interface)
        {
            var RIPEntry = new RIPEntry(Interface, Interface.IPNetwork, null, 1);
            RIPEntry.SyncWithRT = false;
            RIPEntry.AllowUpdates = false;

            Interfaces.Add(Interface);
            RIPTable.Instance.Add(RIPEntry);

            RIPResponse.SendTriggeredUpdate(Interface, RIPEntry);
        }

        static public void Remove(Interface Interface)
        {
            var RIPEntry = RIPTable.Instance.Find(Interface, Interface.IPNetwork);
            RIPEntry.PossibblyDown = true;

            RIPResponse.SendTriggeredUpdate(Interface, RIPEntry);

            Interfaces.Remove(Interface);
            RIPTable.Instance.Remove(RIPEntry);
        }

        static public List<Interface> GetInterfaces()
        {
            return Interfaces.ToList();
        }

        static public void SendUnsolicitedUpdates()
        {
            do
            {
                var Interfaces = GetInterfaces();
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
