using System.Net.NetworkInformation;

namespace Router.Protocols.DHCPOptions
{
    class DHCPClientIdentifierOption : DHCPOption
    {
        private byte IDType;
        private byte[] IDValue;

        public DHCPClientIdentifierOption(byte[] Bytes) : base(DHCPOptionCode.ClientIdentifier)
        {
            // TODO: Bad Practices
            IDType = Bytes[0];
            IDValue = new byte[Length - 1];
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
            IDType = 1;
            IDValue = PhysicalAddress.GetAddressBytes();
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
