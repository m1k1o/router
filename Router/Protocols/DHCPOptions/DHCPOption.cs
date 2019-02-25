using System;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPOption
    {
        public DHCPOptionCode Type { get; private set; }

        public abstract byte[] Bytes { get; }

        public abstract void Parse(string String);

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

        public static DHCPOption Factory(byte OptionType)
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

            return (DHCPOption)Activator.CreateInstance(Type, Type.EmptyTypes);
        }
    }
}
