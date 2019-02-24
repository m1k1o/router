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

        private static object InterfaceServices(Interface Interface)
        {
            return Services;
        }

        private static object Interface(Interface Interface)
        {
            return Interface;
        }

        public static object AvailableServices(string Data = null)
        {
            dynamic services = new { };
            foreach (var Service in Services)
            {
                services[Service.Name] = new
                {
                    description = Service.Description,
                    only_running_interface = Service.OnlyRunningInterface,
                    default_running = Service.DefaultRunning,
                    anonymous = Service.Anonymous
                };
            }
            return services;
        }
        /*
        public static old_JSON ToggleService(string Data = null)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 2)
            {
                return new old_JSONError("Expected InterfaceID, ServiceName.");
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
                return new old_JSONError(e.Message);
            }

            var obj = new old_JSONObject();
            obj.Add("interface", Interface.ID.ToString());
            obj.Add("service", ServiceName);
            obj.Add("status", Interface.ServiceRunning(ServiceName));
            return obj;
        }

        public static old_JSON Toggle(string Data)
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

                return new old_JSONObject("running", Interface.Running);
            }
            catch (Exception e)
            {
                return new old_JSONError(e.Message);
            }
        }

        public static old_JSON Edit(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 3)
            {
                return new old_JSONError("Expected InterfaceID, IPAddress, IPSubnetMask.");
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
                return new old_JSONError(e.Message);
            }

            return Interfaces.Interface(Interface);
        }
        */
        public static object Table(string Data = null)
        {
            dynamic obj = new { };

            var Interfaces = Instance.GetInteraces();
            foreach(var Iface in Interfaces)
            {
                obj[Iface.ID.ToString()] = Interface(Iface);
            }

            return obj;
        }

        public static object Initialize(string Data = null)
        {
            return new
            {
                table = Table(),
                services = AvailableServices()
            };
        }
    }
}
