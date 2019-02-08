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

        internal static List<Interface> Instance { get; } = new List<Interface>();

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

        static public void Add(int ID)
        {
            Instance.Add(Interfaces.Instance.GetInterfaceById(ID));

            // Add C to RIP table
        }

        static public void Remove(int ID)
        {
            Instance.RemoveAll(Interface => Interface.ID == ID);

            // Remove C from RIP table
        }

        static public void SendUnsolicitedUpdates()
        {
            do
            {
                foreach (var Interface in Instance)
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
