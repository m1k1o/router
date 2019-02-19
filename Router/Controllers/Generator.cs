using System;
using PacketDotNet;
using System.Reflection;
using Router.Helpers;
using System.Linq;

namespace Router.Controllers
{
    static class Generator
    {
        public static JSON Send(string Data)
        {
           var Rows = Data.Split('\n');

           // Validate
           if (Rows.Length < 3)
           {
               return new JSONError("Expected InterfaceID, GeneratorType, [Packet].");
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
               ConstructorInfo Constructor = GeneratorType.GetConstructor(null);
               var Generator = ((Router.Generator.Generator)Constructor.Invoke(null));

               // Fill Generator
               Data = string.Join("\n", Rows.Skip(i).ToArray());
               Generator.Parse(Data);

               // Get Packet
               PacketDotNet.Packet Packet = Generator.Export();

               // Send
               Interface.SendPacket(Packet.Bytes);
               return new JSONObject("sent", true);
           }
           catch (Exception e)
           {
               return new JSONError(e.Message);
           }
        }
    }
}
