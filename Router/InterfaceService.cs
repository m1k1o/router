namespace Router
{
    interface InterfaceService
    {
        string Name { get; set; }

        string Description { get; set; }

        void OnStarted(Interface Interface);

        void OnStopped(Interface Interface);

        void OnChanged(Interface Interface);
    }
}
