using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Router.Helpers.JSONConversters;

namespace Router.Helpers
{
    static class JSON
    {
        private static JsonSerializerSettings Settings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new IPAddressConverter());
            settings.Converters.Add(new PhysicalAddressConverter());
            settings.Converters.Add(new IPNetworkConverter());
            settings.Converters.Add(new InterfaceConverter());
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false
                }
            };
            return settings;
        }
        public static JObject Error(string Message)
        {
            return new JObject
            {
                ["error"] = Message
            };
        }

        public static JObject ParseObject(object Object)
        {
            return JObject.FromObject(Object, JsonSerializer.Create(JSON.Settings()));
        }

        public static string SerializeObject(object Object)
        {
            return JsonConvert.SerializeObject(Object, JSON.Settings());
        }

        public static T DeserializeObject<T>(string String)
        {
            return JsonConvert.DeserializeObject<T>(String, JSON.Settings());
        }
        public static T DeserializeObject<T>(string String, T AnonymousType)
        {
            return JsonConvert.DeserializeObject<T>(String, JSON.Settings());
        }
    }
}
