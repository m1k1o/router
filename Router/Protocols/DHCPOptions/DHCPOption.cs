using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPOption
    {
        public static Type GetType(DHCPOptionCode OptionType) => Factory((byte)OptionType);

        [JsonConverter(typeof(StringEnumConverter))] // Serialize enums by name rather than numerical value
        public DHCPOptionCode Type { get; private set; }

        [JsonIgnore]
        public abstract byte[] Bytes { get; }

        public DHCPOption(DHCPOptionCode DHCPOptionCode)
        {
            Type = DHCPOptionCode;
        }

        public static DHCPOption Factory(byte OptionType, byte[] OptionValue = null)
        {
            var OptionName = ((DHCPOptionCode)OptionType).ToString();
            var OptionFound = OptionName != OptionType.ToString();
            if (!OptionFound)
            {
                return new DHCPUnknownOption((DHCPOptionCode)OptionType, OptionValue);
            }

            Type Type = Type.GetType("Router.Protocols.DHCPOptions.DHCP" + OptionName + "Option");
            if (Type == null)
            {
                throw new Exception("Option class not found.");
            }

            return (DHCPOption)Activator.CreateInstance(Type, new object[] { OptionValue });
        }

        public static Type Factory(byte OptionType)
        {
            var OptionName = ((DHCPOptionCode)OptionType).ToString();
            var OptionFound = OptionName != OptionType.ToString();
            if (!OptionFound)
            {
                throw new Exception("Unknown Option '" + OptionName + "'");
            }

            Type Type = Type.GetType("Router.Protocols.DHCPOptions.DHCP" + OptionName + "Option");
            if (Type == null)
            {
                throw new Exception("Option class not found.");
            }

            return Type;
        }
    }
}
