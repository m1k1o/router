using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Net;

namespace Router.Controllers
{
    static class Interfaces
    {
        private static readonly Router.Interfaces Instance = Router.Interfaces.Instance;
        private static readonly List<InterfaceService> Services = Router.Interface.GetAvailableServices();

        private static JSON InterfaceServices(Interface Interface)
        {
            var obj = new JSONObject();
            foreach (var Service in Services)
            {
                obj.Push(Service.Name, Interface.ServiceRunning(Service.Name));
            }
            return obj;
        }

        private static JSON Interface(Interface Interface)
        {
            var obj = new JSONObject();
            //obj.Push("id", Interface.ID);
            obj.Push("name", Interface.Name);
            obj.Push("friendly_name", Interface.FriendlyName);
            obj.Push("description", Interface.Description);
            obj.Push("running", Interface.Running);
            obj.Push("ip", Interface.IPAddress);
            obj.Push("mask", Interface.IPNetwork is IPNetwork ? Interface.IPNetwork.SubnetMask : null);
            obj.Push("mac", Interface.PhysicalAddress);
            obj.Push("services", InterfaceServices(Interface));
            return obj;
        }

        public static JSON AvailableServices(string Data = null)
        {
            var services = new JSONObject();
            var obj = new JSONObject();

            foreach (var Service in Services)
            {
                obj.Empty();
                
                obj.Push("description", Service.Description);
                obj.Push("only_running_interface", Service.OnlyRunningInterface);
                obj.Push("default_running", Service.DefaultRunning);
                obj.Push("anonymous", Service.Anonymous);

                services.Push(Service.Name, obj);
            }

            return services;
        }

        public static JSON ToggleService(string Data = null)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 2)
            {
                return new JSONError("Expected InterfaceID, ServiceName.");
            }

            Interface Interface;
            String ServiceName = Rows[1];
            try
            {
                Interface = Instance.GetInterfaceById(Rows[0]);
                Interface.ServiceToggle(ServiceName);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            var obj = new JSONObject();
            obj.Push("interface", Interface.ID.ToString());
            obj.Push("service", ServiceName);
            obj.Push("status", Interface.ServiceRunning(ServiceName));
            return obj;
        }

        public static JSON Toggle(string Data)
        {
            try
            {
                var Interface = Instance.GetInterfaceById(Data);
                if (!Interface.Running)
                {
                    Interface.Start();
                }
                else
                {
                    Interface.Stop();
                }

                return new JSONObject("running", Interface.Running);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }
        }

        public static JSON Edit(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 3)
            {
                return new JSONError("Expected InterfaceID, IPAddress, IPSubnetMask.");
            }

            Interface Interface;
            try
            {
                Interface = Instance.GetInterfaceById(Rows[0]);

                var IP = IPAddress.Parse(Rows[1]);
                var SubnetMask = IPSubnetMask.Parse(Rows[2]);
                Interface.SetIP(IP, SubnetMask);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            return Interfaces.Interface(Interface);
        }
        
        public static JSON Table(string Data = null)
        {
            var obj = new JSONObject();

            var Interfaces = Instance.GetInteraces();
            foreach(var Iface in Interfaces)
            {
                obj.Push(Iface.ID.ToString(), Interface(Iface));
            }

            return obj;
        }

        public static JSON Initialize(string Data = null)
        {
            var obj = new JSONObject();
            obj.Push("table", Table());
            obj.Push("services", AvailableServices());
            return obj;
        }
    }
}
