using Newtonsoft.Json;
using System;

namespace Router.Helpers.JSONConverters
{
    class InterfaceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Interface));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((Interface)value).ID.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return Interfaces.Instance.GetInterfaceById(reader.Value.ToString());
            }
            catch (Exception)
            {
                return null;
                //throw new JsonSerializationException("Invalid Interface.");
            }
        }
    }
}
