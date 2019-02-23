using Newtonsoft.Json;

namespace Router.Helpers.Json
{
    class JSONObject
    {
        private static JsonSerializerSettings Settings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new IPAddressConverter());
            settings.Converters.Add(new PhysicalAddressConverter());
            settings.Converters.Add(new IPNetworkConverter());
            settings.Formatting = Formatting.Indented;

            return settings;
        }

        public static string Serialize(object Object)
        {
            return JsonConvert.SerializeObject(Object, Settings());
        }

        public static T Deserialize<T>(string String)
        {
            return JsonConvert.DeserializeObject<T>(String, Settings());
        }
    }
}
