using Router.LLDP;
using System.Collections.Generic;

namespace Router.Controllers.LLDP
{
    class Table : Controller
    {
        public static object Entry(LLDPEntry LLDPEntry) => new
        {
            //id = LLDPEntry.ID,
            chassis_id = LLDPEntry.ChassisID.SubTypeValue,
            port_id = LLDPEntry.PortID.SubTypeValue,
            time_to_live = LLDPEntry.ExpiresIn,
            port_description = LLDPEntry.PortDescription?.StringValue,
            system_name = LLDPEntry.SystemName?.StringValue,
            //system_description = LLDPEntry.SystemDescription.StringValue,
            //system_capabilities = LLDPEntry.SystemCapabilities.ToString(),
            //management_address = LLDPEntry.ManagementAddress.ToString(),
            //organization_specific = LLDPEntry.OrganizationSpecific.ToString(),
            Interface = LLDPEntry.Interface.ID
        };

        public object Export()
        {
            var LLDPEntries = LLDPTable.GetEntries();

            var List = new List<object>();
            foreach (var LLDPEntry in LLDPEntries)
            {
                List.Add(Entry(LLDPEntry));
            }

            return List;
        }
    }
}
