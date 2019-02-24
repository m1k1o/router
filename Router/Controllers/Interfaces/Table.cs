using System.Collections.Generic;

namespace Router.Controllers.Interfaces
{
    class Table : Controller
    {
        private static Dictionary<string, bool> InterfaceServices(Interface Interface)
        {
            var Services = Interface.GetAvailableServices();

            var Dictionary = new Dictionary<string, bool>();
            foreach (var Service in Services)
            {
                Dictionary.Add(Service.Name, Interface.ServiceRunning(Service.Name));
            }
            return Dictionary;
        }

        public object Export()
        {
            var Interfaces = Router.Interfaces.Instance.GetInteraces();

            var Dictionary = new Dictionary<string, object>();
            foreach (var Interface in Interfaces)
            {
                Dictionary.Add(Interface.ID.ToString(), new {
                    name = Interface.Name,
                    friendly_name = Interface.FriendlyName,
                    description = Interface.Description,
                    running = Interface.Running,
                    ip = Interface.IPAddress,
                    //network = Interface.IPNetwork,
                    mask = Interface.IPNetwork?.SubnetMask,
                    mac = Interface.PhysicalAddress,
                    services = InterfaceServices(Interface)
                });
            }

            return Dictionary;
        }
    }
}
