using System;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPOption
    {
        public DHCPOptionCode Type { get; private set; }

        public virtual byte Length => (byte)Bytes.Length;

        public abstract byte[] Bytes { get; }

        public DHCPOption(DHCPOptionCode DHCPOptionCode)
        {
            Type = DHCPOptionCode;
        }

        public static DHCPOption Factory(byte OptionType, byte[] OptionValue)
        {
            var OptionName = ((DHCPOptionCode)OptionType).ToString();
            var OptionFound = OptionName != OptionType.ToString();
            if (!OptionFound)
            {
                return null;
            }

            Type Type = Type.GetType("Router.Protocols.DHCPOptions.DHCP" + OptionName + "Option");
            if (Type == null)
            {
                throw new Exception("Option class not found.");
            }

            return (DHCPOption)Activator.CreateInstance(Type, new object[] { OptionValue });
        }
    }
}
