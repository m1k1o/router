using System;

namespace Router.Protocols.DHCPOptions
{
    class DHCPMessageTypeOption : DHCPOption
    {
        public byte MessageType { get; private set; }

        public DHCPMessageTypeOption() : base(DHCPOptionCode.MessageType) { }

        public DHCPMessageTypeOption(byte[] Bytes) : base(DHCPOptionCode.MessageType)
        {
            MessageType = Bytes[0];
        }

        public DHCPMessageTypeOption(DHCPMessageType MessageType) : base(DHCPOptionCode.MessageType)
        {
            this.MessageType = (byte)MessageType;
        }

        public override byte[] Bytes => new byte[] { MessageType };

        public override void Parse(string String)
        {
            MessageType = Convert.ToByte(String);
        }
    }

    enum DHCPMessageType : byte
    {
        // DHCP Discover message
        Discover = 1,
        
        // DHCP Offer message
        Offer = 2,

        // DHCP Request message
        Request = 3,
        
        // DHCP Decline message
        Decline = 4,
        
        // DHCP Acknowledgment message
        Ack = 5,
        
        // DHCP Negative Acknowledgment message
        Nak = 6,
        
        // DHCP Release message
        Release = 7
    }
}
