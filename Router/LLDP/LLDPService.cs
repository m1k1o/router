using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.LLDP
{
    class LLDPService : InterfaceService
    {
        public string Name { get; } = "lldp";

        public string Description { get; } = "LLDP";

        public bool OnlyRunningInterface { get; } = true;

        public void OnStarted(Interface Interface)
        {
            throw new NotImplementedException();
        }

        public void OnStopped(Interface Interface)
        {
            throw new NotImplementedException();
        }

        public void OnChanged(Interface Interface)
        {
            throw new NotImplementedException();
        }

        public void OnPacketArrival(object RawData, Interface Interface)
        {
            throw new NotImplementedException();
        }

        /*

        public static bool Running { get; private set; } = false;

        private static Thread Thread;
        private static int Sleep = 30000; // wait 30sec

        public static void Toggle()
        {
            if (Running)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private static void Start()
        {
            Running = true;

            Thread = new Thread(BackgroundThread);
            Thread.Start();
        }

        private static void Stop()
        {
            Running = false;
            if (!Thread.Join(500))
            {
                Thread.Abort();
            }
        }

        private static void BackgroundThread()
        {
            while (Running)
            {
                // Send to all
                foreach (var Interface in Interfaces.Instance.GetInteraces())
                {
                    try
                    {
                        Send(Interface);
                    }
                    catch { };
                }

                Thread.Sleep(Sleep);
            }
        }
        */
    }
}
