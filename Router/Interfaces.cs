using SharpPcap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Router
{
    class Interfaces
    {
        public static Interfaces Instance { get; } = new Interfaces();

        private List<Interface> Available = new List<Interface>();

        int Index = 0;

        private Interfaces()
        {
            // Print SharpPcap version
            Console.WriteLine("SharpPcap {0}.", SharpPcap.Version.VersionString);
            Console.WriteLine("Loading...");

            foreach (var Device in CaptureDeviceList.Instance)
            {
                DeviceAdd(Device);
            }
        }

        public void Refresh()
        {
            CaptureDeviceList.Instance.Refresh();
            foreach (var Device in CaptureDeviceList.Instance)
            {
                if(!Available.Exists(Interface => Interface.Name == Device.Name))
                {
                    DeviceAdd(Device);
                }
            }
        }

        private void DeviceAdd(ICaptureDevice Device)
        {
            var Interface = new Interface(Device, Index);
            if (Interface.Valid)
            {
                Available.Add(Interface);
                Index++;
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
