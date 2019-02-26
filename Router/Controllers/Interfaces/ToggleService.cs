using System;

namespace Router.Controllers.Interfaces
{
    class ToggleService : Controller, Executable
    {
        public Interface Interface { get; set; }
        public string Service { get; set; }
        public bool? Status => Interface?.ServiceRunning(Service);

        public void Execute()
        {
            if (Interface == null || Service == null)
            {
                throw new Exception("Expected Interface, Service.");
            }

            Interface.ServiceToggle(Service);
        }

        public object Export() => this;
    }
}