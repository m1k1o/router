using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.Controllers
{
    class Sniffing
    {
        public static old_JSON Interface(string Data = null)
        {
            if (!string.IsNullOrEmpty(Data))
            {
                try
                {
                    if (Data == "null")
                    {
                        Router.Sniffing.SniffingList.Interface = null;
                    }
                    else
                    {
                        var Interface = Router.Interfaces.Instance.GetInterfaceById(Data);
                        Router.Sniffing.SniffingList.Interface = Interface;
                    }
                }
                catch (Exception e)
                {
                    return new old_JSONError(e.Message);
                }
            }

            return new old_JSONObject("interface", Router.Sniffing.SniffingList.Interface == null ? null : Router.Sniffing.SniffingList.Interface.ID.ToString());
        }


        public static old_JSON Pop(string Data = null)
        {
            return Router.Sniffing.SniffingList.Pop();
        }

        public static old_JSON Initialize(string Data = null)
        {
            var obj = new old_JSONObject();
            obj.Add("interface", Router.Sniffing.SniffingList.Interface == null ? null : Router.Sniffing.SniffingList.Interface.ID.ToString());
            obj.Add("data", Pop());
            return obj;
        }
    }
}
