using System;

namespace Router
{
    interface InterfaceService
    {
        string Name { get; }

        string Description { get; }

        bool OnlyRunningInterface { get; }

        void OnStarted(Interface Interface);

        void OnStopped(Interface Interface);

        void OnChanged(Interface Interface);

        void OnPacketArrival(Handler Handler);
    }
}
