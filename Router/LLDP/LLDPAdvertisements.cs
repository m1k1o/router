using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Router.LLDP
{
    class LLDPAdvertisements
    {
        public static TimeSpan Timer { get; set; } = TimeSpan.FromSeconds(30);

        private static ManualResetEvent StopRequest = new ManualResetEvent(false);
        private static Thread Thread;

        private static List<Interface> Interfaces = new List<Interface>();

        public static void Add(Interface Interface)
        {
            if (Interfaces.Count == 0)
            {
                StopRequest.Reset();

                Thread = new Thread(SendUpdates);
                Thread.Start();
            }

            Interfaces.Add(Interface);
        }

        public static void Remove(Interface Interface)
        {
            Interfaces.Remove(Interface);

            if (Interfaces.Count == 0)
            {
                StopRequest.Set();
                Thread.Join();
            }
        }

        private static void SendUpdates()
        {
            Console.WriteLine("LLDP thread started...");
            do
            {
                var Ifaces = Interfaces.ToList();
                foreach (var Interface in Ifaces)
                {
                    LLDPResponse.Send(Interface);
                }

                if (StopRequest.WaitOne(Timer))
                {
                    Console.WriteLine("LLDP thread stopped...");
                    break;
                }
            }
            while (true);
        }
    }
}
