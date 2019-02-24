using System.Collections.Generic;

namespace Router.Controllers.Interfaces
{
    class Services : Controller
    {
        public static object Entry(InterfaceService Service) => new
        {
            description = Service.Description,
            only_running_interface = Service.OnlyRunningInterface,
            default_running = Service.DefaultRunning,
            anonymous = Service.Anonymous,
        };

        public object Export()
        {
            var Services = Interface.GetAvailableServices();

            var Dictionary = new Dictionary<string, object>();
            foreach (var Service in Services)
            {
                Dictionary.Add(Service.Name, Entry(Service));
            }

            return Dictionary;
        }
    }
}