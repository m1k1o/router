using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Router.Protocols.DHCPOptions;
using System;

namespace Router.Helpers.JSONConverters
{
    class DHCPOptionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DHCPOption).IsAssignableFrom(objectType);
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var typeToken = token["type"];
            if (typeToken == null)
            {
                throw new InvalidOperationException("Invalid DHCPOption object.");
            }

            var actualType = DHCPOption.GetType(typeToken.ToObject<DHCPOptionCode>(serializer));
            if (existingValue == null || existingValue.GetType() != actualType)
            {
                var contract = serializer.ContractResolver.ResolveContract(actualType);
                existingValue = contract.DefaultCreator();
            }

            using (var subReader = token.CreateReader())
            {
                // Using "populate" avoids infinite recursion.
                serializer.Populate(subReader, existingValue);
            }
            return existingValue;
        }
    }
}
