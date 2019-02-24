using System;
using PacketDotNet;
using System.Reflection;
using Router.Helpers;
using System.Linq;

namespace Router.Controllers
{
    static class Generator
    {
        public static old_JSON Send(string Data)
        {
           var Rows = Data.Split('\n');

           // Validate
           if (Rows.Length < 3)
           {
               return new old_JSONError("Expected InterfaceID, GeneratorType, [Packet].");
           }

           try
           {
               var i = 0;
               var Interface = Router.Interfaces.Instance.GetInterfaceById(Rows[i++]);
               var ClassName = Rows[i++];

               if (!Interface.Running)
               {
                   throw new Exception("Interface '" + Interface + "' is not running.");
               }

               // Get generator
               Type GeneratorType = Type.GetType("Router.Generator." + ClassName);
               if (GeneratorType == null)
               {
                   throw new Exception("GeneratorType '" + ClassName + "' not found.");
               }

               // Create instance
               ConstructorInfo Constructor = GeneratorType.GetConstructor(Type.EmptyTypes);
               var Generator = ((Router.Generator.Generator)Constructor.Invoke(Type.EmptyTypes));

               // Fill Generator
               Generator.Parse(Rows, ref i);

               // Get Packet
               PacketDotNet.Packet Packet = Generator.Export();

               // Send
               Interface.SendPacket(Packet.Bytes);

               var obj = new old_JSONObject();
                obj.Add("interface", Interface);
                obj.Add("packet", Packet);
                return obj;
           }
           catch (Exception e)
           {
               return new old_JSONError(e.Message);
           }
        }
    }
}
