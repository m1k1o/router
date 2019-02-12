using System;
using System.Collections.Generic;

namespace Router.RIP
{
    class RIPService : InterfaceService
    {
        public string Name { get; set; } = "RIP";

        public string Description { get; set; } = "Routing Information Protocol";

        public void OnStarted(Interface Interface)
        {
            var RIPEntry = new RIPEntry(Interface, Interface.IPNetwork, Interface.IPAddress, 1)
            {
                SyncWithRT = false,
                CanBeUpdated = false,
                TimersEnabled = false
            };

            RIPTable.Instance.Add(RIPEntry);

            RIPUpdates.SendTriggered(Interface, RIPEntry);

            RIPRequest.AskForUpdate(Interface);
        }

        public void OnStopped(Interface Interface)
        {
            var RIPEntry = RIPTable.Instance.Find(Interface, Interface.IPNetwork);
            if (RIPEntry == null)
            {
                throw new Exception("RIPEntry not found while Stopping");
            }

            RIPEntry.PossibblyDown = true;

            RIPUpdates.SendTriggered(Interface, RIPEntry);
            
            RIPTable.Instance.Remove(Interface);

            RIPTable.Instance.SyncWithRT();
        }

        public void OnChanged(Interface Interface)
        {
            throw new NotImplementedException();

            var RIPEntry = RIPTable.Instance.Find(Interface, Interface.IPNetwork);
            if (RIPEntry == null)
            {
                throw new Exception("RIPEntry not found while Updating");
            }

            RIPUpdates.SendTriggered(Interface, RIPEntry);

            RIPTable.Instance.SyncWithRT();
        }
    }
}
