namespace Router.Services
{
    class RIPService : InterfaceService
    {
        public string Name { get; set; } = "RIP";

        public string Description { get; set; } = "Routing Information Protocol";

        public void OnStarted(Interface Interface)
        {

        }

        public void OnStopped(Interface Interface)
        {

        }

        public void OnChanged(Interface Interface)
        {

        }
    }
}
