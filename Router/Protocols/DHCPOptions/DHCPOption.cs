using Newtonsoft.Json;
using System;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPOption
    {
        public static Type GetType(DHCPOptionCode OptionType) => Factory((byte)OptionType);

        public DHCPOptionCode Type { get; set; }
        // TODO: Integrity issue: set should be private, but UnknownOption must be supported as well

        [JsonIgnore]
        public abstract byte[] Bytes { get; }

        public DHCPOption(DHCPOptionCode DHCPOptionCode)
        {
            Type = DHCPOptionCode;
        }

        public static DHCPOption Factory(byte OptionType, byte[] OptionValue = null)
        {
            // TODO: Refactor
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
            // TODO: Refactor
            var OptionName = ((DHCPOptionCode)OptionType).ToString();
            var OptionFound = OptionName != OptionType.ToString();
            if (!OptionFound)
            {
                return typeof(DHCPUnknownOption);
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
