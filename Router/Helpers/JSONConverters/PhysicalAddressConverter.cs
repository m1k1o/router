using Newtonsoft.Json;
using System;
using System.Net.NetworkInformation;

namespace Router.Helpers.JSONConverters
{
    class PhysicalAddressConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(PhysicalAddress));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var PhysicalAddress = (PhysicalAddress)value;
            var FormattedMAC = BitConverter.ToString(PhysicalAddress.GetAddressBytes()).Replace("-", ":");
            writer.WriteValue(FormattedMAC);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var FormattedMAC = (string)reader.Value;
                return PhysicalAddress.Parse(FormattedMAC.ToUpper().Replace(":", "-"));
            }
            catch (Exception)
            {
                return null;
                //throw new JsonSerializationException("Invalid PhysicalAddress.");
            }
        }
    }
}
