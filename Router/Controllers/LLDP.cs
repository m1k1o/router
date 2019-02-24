using Router.Helpers;
using Router.LLDP;
using System;

namespace Router.Controllers
{
    class LLDP
    {
        private static old_JSON LLDPEntry(LLDPEntry LLDPEntry)
        {
            var obj = new old_JSONObject();
            //obj.Push("id", LLDPEntry.ID);
            obj.Add("chassis_id", LLDPEntry.ChassisID.SubTypeValue);
            obj.Add("port_id", LLDPEntry.PortID.SubTypeValue);
            obj.Add("time_to_live", LLDPEntry.ExpiresIn);
            obj.Add("port_description", LLDPEntry.PortDescription == null ? null : LLDPEntry.PortDescription.StringValue);
            obj.Add("system_name", LLDPEntry.SystemName == null ? null : LLDPEntry.SystemName.StringValue);
            //obj.Push("system_description", LLDPEntry.SystemDescription.StringValue);
            //obj.Push("system_capabilities", LLDPEntry.SystemCapabilities.ToString());
            //obj.Push("management_address", LLDPEntry.ManagementAddress.ToString());
            //obj.Push("organization_specific", LLDPEntry.OrganizationSpecific.ToString());
            obj.Add("interface", LLDPEntry.Interface.ID);
            return obj;
        }

        public static old_JSON Settings(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var Rows = Data.Split('\n');

                // Validate
                if (Rows.Length != 4)
                {
                    return new old_JSONError("Expected AdvertisementsInterval, TimeToLive, SystemName, SystemDescription.");
                }

                try
                {
                    var Timer = TimeSpan.FromSeconds(Int32.Parse(Rows[0]));
                    var TimeToLive = UInt16.Parse(Rows[1]);

                    LLDPAdvertisements.Timer = Timer;
                    LLDPResponse.TimeToLive = TimeToLive;
                    LLDPResponse.SystemName = Rows[2];
                    LLDPResponse.SystemDescription = Rows[3];
                }
                catch (Exception e)
                {
                    return new old_JSONError(e.Message);
                }
            }

            var obj = new old_JSONObject();
            obj.Add("adv_interval", LLDPAdvertisements.Timer.TotalSeconds);
            obj.Add("time_to_live", LLDPResponse.TimeToLive);
            obj.Add("system_name", LLDPResponse.SystemName);
            obj.Add("system_description", LLDPResponse.SystemDescription);
            return obj;
        }

        public static old_JSON Table(string Data = null)
        {
            //var obj = new JSONObject();
            var arr = new old_JSONArray();

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

        public static old_JSON Initialize(string Data = null)
        {
            var obj = new old_JSONObject();
            obj.Add("table", Table());
            obj.Add("settings", Settings());
            return obj;
        }
    }
}
