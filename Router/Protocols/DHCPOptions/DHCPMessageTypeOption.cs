namespace Router.Protocols.DHCPOptions
{
    class DHCPMessageTypeOption : DHCPOption
    {
        public DHCPMessageType MessageType { get; set; }

        public DHCPMessageTypeOption() : base(DHCPOptionCode.MessageType)
        {
            MessageType = DHCPMessageType.None;
        }

        public DHCPMessageTypeOption(byte[] Bytes) : base(DHCPOptionCode.MessageType)
        {
            MessageType = (DHCPMessageType)Bytes[0];
        }

        public DHCPMessageTypeOption(DHCPMessageType MessageType) : base(DHCPOptionCode.MessageType)
        {
            this.MessageType = MessageType;
        }

        public override byte[] Bytes => new byte[] { (byte)MessageType };
    }

    enum DHCPMessageType : byte
    {
        // None
        None = 0,

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
