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

        private Interfaces()
        {
            // Print SharpPcap version
            Console.WriteLine("SharpPcap {0}.", SharpPcap.Version.VersionString);
            Console.WriteLine("Loading...");

            var i = 0;
            foreach (var Device in CaptureDeviceList.Instance)
            {
                var Interface = new Interface(Device, i);
                if(Interface.Valid)
                {
                    Available.Add(Interface);
                    i++;
                }
            }
        }

        public Interface GetInterfaceById(string ID)
        {
            return Available[Int32.Parse(ID)];
        }

        public List<Interface> GetInteraces()
        {
            return Available.ToList();
        }
    }
}
