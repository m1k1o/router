namespace Router.Sniffing
{
    class SniffingService : InterfaceService
    {
        public string Name { get; } = "sniffing";

        public string Description { get; } = "Sniffing";

        public bool OnlyRunningInterface { get; } = true;

        public bool DefaultRunning { get; } = true;

        public bool Anonymous { get; } = true;

        public void OnStarted(Interface Interface) { }

        public void OnStopped(Interface Interface) { }

        public void OnChanged(Interface Interface) { }

        public void OnPacketArrival(Handler Handler)
        {
            if (Equals(Handler.Interface, SniffingList.Interface))
            {
                var SniffingJSON = new SniffingJSON(Handler);
                SniffingJSON.Extract();
                SniffingList.Push(SniffingJSON.Result);
            }
        }
    }
}
