using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Router.Packets;
using System;

namespace Router.Helpers.JSONConverters
{
    class IGeneratorPacketConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IGeneratorPacket).IsAssignableFrom(objectType);
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var PacketType = jObject["key"].Value<string>();

            object target = Generator.Factory(PacketType);

            serializer.Populate(jObject["value"].CreateReader(), target);

            return target;
        }
    }
}
