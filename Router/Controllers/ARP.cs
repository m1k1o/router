using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.Controllers
{
    class ARP
    {
        static private readonly ARPTable ARPTable = ARPTable.Instance;

        public JSON TTL(string Data = null)
        {
            if (string.IsNullOrEmpty(Data))
            {
                // Answer
                return new JSONObject("ttl", ARPTable.TTL);
            }

            // Set new TTL
            try
            {
                ARPTable.TTL = Int32.Parse(Data);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            // Answer
            return new JSONObject("ttl", ARPTable.TTL);
        }

        public JSON Lookup(string Data)
        {
            var Rows = Data.Split('\n');

            // Validate
            if (Rows.Length != 2)
            {
                return new JSONError("Expected InterfaceID, IPAddress.");
            }

            Interface Interface;
            IPAddress IPAddress;
            try
            {
                Interface = Router.Interfaces.Instance.GetInterfaceById(Rows[0]);
                IPAddress = IPAddress.Parse(Rows[1]);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            if (!Interface.Running)
            {
                return new JSONError("Interface must be running.");
            }

            // Action
            var MAC = Router.ARP.Lookup(IPAddress, Interface);

            // Answer
            return new JSONObject("mac", MAC);
        }

        public JSON Table(string Data = null)
        {
            long timeStamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

            var arr = new JSONArray();
            var obj = new JSONObject();

            var Rows = ARPTable.GetEntries();
            foreach (var Row in Rows)
            {
                var TTL = Row.TTL - timeStamp;
                if (TTL < 0)
                {
                    continue;
                }

                obj.Empty();

                obj.Push("ip", Row.IPAddress);
                obj.Push("mac", Row.PhysicalAddress);
                obj.Push("ttl", TTL);

                arr.Push(obj);
            }

            return arr;
        }
    }
}
