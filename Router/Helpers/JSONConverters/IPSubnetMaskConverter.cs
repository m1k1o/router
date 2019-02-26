using Newtonsoft.Json;
using System;
using System.Net;

namespace Router.Helpers.JSONConverters
{
    class IPSubnetMaskConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPSubnetMask));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return IPSubnetMask.Parse((string)reader.Value);
            }
            catch (Exception)
            {
                return null;
                //throw new JsonSerializationException("Invalid IPSubnetMask.");
            }
        }
    }
}
