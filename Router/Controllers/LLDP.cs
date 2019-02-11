using Router.Helpers;
using Router.LLDP;
using System;

namespace Router.Controllers
{
    class LLDP
    {
        private static JSON LLDPEntry(LLDPEntry LLDPEntry)
        {
            var obj = new JSONObject();
            //obj.Push("id", LLDPEntry.ID);
            obj.Push("chassis_id", LLDPEntry.ChassisID);
            obj.Push("port_id", LLDPEntry.PortID);
            obj.Push("time_to_live", LLDPEntry.TimeToLive);
            obj.Push("port_description", LLDPEntry.PortDescription);
            obj.Push("system_name", LLDPEntry.SystemName);
            obj.Push("system_description", LLDPEntry.SystemDescription);
            obj.Push("system_capabilities", LLDPEntry.SystemCapabilities);
            obj.Push("management_address", LLDPEntry.ManagementAddress);
            obj.Push("organization_specific", LLDPEntry.OrganizationSpecific);
            return obj;
        }

        public static JSON Settings(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 4)
                {
                    return new JSONError("Expected Running, TimeToLive, SystemName, SystemDescription.");
                }

                try
                {
                    if(LLDPProcess.Running != (Rows[0] == "true"))
                    {
                        LLDPProcess.Toggle();
                    }

                    LLDPProcess.TimeToLive = UInt16.Parse(Rows[1]);
                    LLDPProcess.SystemName = Rows[2];
                    LLDPProcess.SystemDescription = Rows[3];
                }
                catch (Exception e)
                {
                    return new JSONError(e.Message);
                }
            }

            var obj = new JSONObject();
            obj.Push("running", LLDPProcess.Running);
            obj.Push("time_to_live", LLDPProcess.TimeToLive);
            obj.Push("system_name", LLDPProcess.SystemName);
            obj.Push("system_decsription", LLDPProcess.SystemDescription);
            return obj;
        }

        public static JSON Table(string Data = null)
        {
            var obj = new JSONObject();

            var Rows = LLDPTable.GetEntries();
            foreach (var Row in Rows)
            {
                if (Row.HasExpired)
                {
                    continue;
                }

                obj.Push(Row.ID.ToString(), LLDPEntry(Row));
            }

            return obj;
        }

        public static JSON Initialize(string Data = null)
        {
            var obj = new JSONObject();
            obj.Push("table", Table());
            obj.Push("settings", Settings());
            return obj;
        }
    }
}
