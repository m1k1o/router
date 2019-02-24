using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Router.Helpers.JSONConverters;

namespace Router.Helpers
{
    static class JSON
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters = {
                    new IPAddressConverter(),
                    new PhysicalAddressConverter(),
                    new IPNetworkConverter(),
                    new InterfaceConverter()
                },
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false
                }
            }
        };

        public static JObject Error(string Message)
        {
            return new JObject
            {
                ["error"] = Message
            };
        }

        public static void PopulateObject(string String, object Object)
        {
            JsonConvert.PopulateObject(String, Object, Settings);
        }

        public static string SerializeObject(object Object)
        {
            return JsonConvert.SerializeObject(Object, Settings);
        }
    }
}
