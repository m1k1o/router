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
                if (Rows.Length != 3)
                {
                    return new JSONError("Expected TimeToLive, SystemName, SystemDescription.");
                }

                try
                {
                    LLDPResponse.TimeToLive = UInt16.Parse(Rows[0]);
                    LLDPResponse.SystemName = Rows[1];
                    LLDPResponse.SystemDescription = Rows[2];
                }
                catch (Exception e)
                {
                    return new JSONError(e.Message);
                }
            }

            var obj = new JSONObject();
            obj.Push("time_to_live", LLDPResponse.TimeToLive);
            obj.Push("system_name", LLDPResponse.SystemName);
            obj.Push("system_description", LLDPResponse.SystemDescription);
            return obj;
        }

        public static JSON Table(string Data = null)
        {
            //var obj = new JSONObject();
            var arr = new JSONArray();

            var Rows = LLDPTable.GetEntries();
            foreach (var Row in Rows)
            {
                if (Row.HasExpired)
                {
                    continue;
                }

                //obj.Push(Row.ID.ToString(), LLDPEntry(Row));
                arr.Push(LLDPEntry(Row));
            }

            //return obj;
            return arr;
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
