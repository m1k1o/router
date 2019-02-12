using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Router.RIP
{
    static class RIPUpdates
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
            do
            {
                Send();

                if (StopRequest.WaitOne(Timer))
                {
                    Console.WriteLine("Terminating thread...");
                    break;
                }
            }
            while (true);
        }

        public static void Send()
        {
            foreach (var Interface in Interfaces)
            {
                var RIPResponse = new RIPResponse(Interface);
                RIPResponse.Send();
            }
        }

        public static void SendTriggered(Interface SourceInterface, List<RIPEntry> RIPEntries)
        {
            foreach (var Interface in Interfaces)
            {
                if (Equals(Interface, SourceInterface))
                {
                    continue;
                }

                var RIPResponse = new RIPResponse(Interface);
                RIPResponse.Send(RIPEntries);
            }
        }

        public static void SendTriggered(Interface SourceInterface, RIPEntry RIPEntry)
        {
            SendTriggered(SourceInterface, new List<RIPEntry> { RIPEntry });
        }
    }
}
