using Router.Helpers;
using System.Net.NetworkInformation;

namespace Router.Protocols.DHCPOptions
{
    class DHCPClientIdentifierOption : DHCPOption
    {
        public PhysicalAddress PhysicalAddress
        {
            get
            {
                try
                {
                    return new PhysicalAddress(IDValue);
                } catch
                {
                    return null;
                }
            }
            set
            {
                IDType = 1;
                IDValue = value.GetAddressBytes();
            }
        }

        public byte IDType { get; set; } = 0;
        public byte[] IDValue { get; set; } = new byte[0];

        public DHCPClientIdentifierOption() : base(DHCPOptionCode.ClientIdentifier)
        {
            IDType = 1;
            IDValue = new byte[0];
        }

        public DHCPClientIdentifierOption(byte[] Bytes) : base(DHCPOptionCode.ClientIdentifier)
        {
            // TODO: Bad Practices
            IDType = Bytes[0];
            IDValue = new byte[Bytes.Length - 1];
            for (var i = 1; i < Bytes.Length; i++)
            {
                IDValue[i - 1] = Bytes[i];
            }
        }

        public DHCPClientIdentifierOption(byte IDType, byte[] IDValue) : base(DHCPOptionCode.ClientIdentifier)
        {
            this.IDType = IDType;
            this.IDValue = IDValue;
        }

        public DHCPClientIdentifierOption(PhysicalAddress PhysicalAddress) : base(DHCPOptionCode.ClientIdentifier)
        {
            this.PhysicalAddress = PhysicalAddress;
        }

        public override byte[] Bytes
        {
            get
            {
                // TODO: Bad Practices
                var RawData = new byte[IDValue.Length + 1];
                RawData[0] = IDType;
                for (var i = 1; i < IDValue.Length; i++)
                {
                    RawData[i] = IDValue[i - 1];
                }
                return RawData;
            }
        }
    }
}
