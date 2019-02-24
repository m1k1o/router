using System.Collections.Generic;

namespace Router.Controllers.Interfaces
{
    class Services : Controller
    {
        public object Export()
        {
            var Services = Interface.GetAvailableServices();

            var Dictionary = new Dictionary<string, object>();
            foreach (var Service in Services)
            {
                Dictionary.Add(Service.Name, Service);
            }

            return Dictionary;
        }
    }
}