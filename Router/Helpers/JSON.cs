﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Router.Helpers.JSONConverters;

namespace Router.Helpers
{
    static class JSON
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters =
            {
                // Primitive objects
                new IPAddressConverter(),
                new PhysicalAddressConverter(),
                new IPNetworkConverter(),
                new IPSubnetMaskConverter(),

                // Custom objects
                new InterfaceConverter(),
                new GeneratorPacketConverter(),
                new DHCPOptionConverter(),
                new TestCaseConverter()
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

        public static T DeserializeAnonymousType<T>(string String, T Definition)
        {
            return JsonConvert.DeserializeAnonymousType<T>(String, Definition, Settings);
        }

        public static string SerializeObject(object Object)
        {
            return JsonConvert.SerializeObject(Object, Settings);
        }
    }
}
