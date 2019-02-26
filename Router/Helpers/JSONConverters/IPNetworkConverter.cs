using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Router.Helpers.JSONConverters
{
    class IPNetworkConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPNetwork));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IPNetwork IPNetwork = (IPNetwork)value;
            JObject JObject = new JObject
            {
                { "short", IPNetwork.ToString() },
                { "ip", IPNetwork.NetworkAddress.ToString() },
                { "mask", IPNetwork.SubnetMask.ToString() }
            };
            JObject.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject JObject = JObject.Load(reader);
                return IPNetwork.Parse(JObject["ip"].ToString(), JObject["mask"].ToString());
            }
            catch (Exception)
            {
                return null;
                //throw new JsonSerializationException("Invalid IPNetwork.");
            }
        }
    }
}
