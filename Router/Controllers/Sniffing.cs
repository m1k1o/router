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
        public static JSON Interface(string Data = null)
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
                    return new JSONError(e.Message);
                }
            }

            return new JSONObject("interface", Router.Sniffing.SniffingList.Interface == null ? null : Router.Sniffing.SniffingList.Interface.ID.ToString());
        }


        public static JSON Pop(string Data = null)
        {
            return Router.Sniffing.SniffingList.Pop();
        }

        public static JSON Initialize(string Data = null)
        {
            var obj = new JSONObject();
            obj.Push("interface", Router.Sniffing.SniffingList.Interface == null ? null : Router.Sniffing.SniffingList.Interface.ID.ToString());
            obj.Push("data", Pop());
            return obj;
        }
    }
}
