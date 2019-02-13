using Router.Helpers;
using System.Net;

namespace Router.Protocols
{
    sealed class DHCPPacket : Packet
    {
        public byte OperationCode
        {
            get => (byte)Slice(0, typeof(byte));
            set => Inject(0, value);
        }

        public byte HardwareType
        {
            get => (byte)Slice(1, typeof(byte));
            set => Inject(1, value);
        }

        public byte HardwareAddressLength
        {
            get => (byte)Slice(2, typeof(byte));
            set => Inject(2, value);
        }

        public byte Hops
        {
            get => (byte)Slice(3, typeof(byte));
            set => Inject(3, value);
        }

        public uint TransactionIdentifier
        {
            get => (uint)Slice(4, typeof(uint));
            set => Inject(4, value);
        }

        public ushort Seconds
        {
            get => (ushort)Slice(8, typeof(ushort));
            set => Inject(8, value);
        }

        public ushort Flags
        {
            get => (ushort)Slice(10, typeof(ushort));
            set => Inject(10, value);
        }

        public IPAddress ClientIPAddress
        {
            get => (IPAddress)Slice(12, typeof(IPAddress));
            set => Inject(12, value);
        }

        public IPAddress YourIPAddress
        {
            get => (IPAddress)Slice(16, typeof(IPAddress));
            set => Inject(16, value);
        }

        public IPAddress ServerIPAddress
        {
            get => (IPAddress)Slice(20, typeof(IPAddress));
            set => Inject(20, value);
        }

        public IPAddress GatewayIPAddress
        {
            get => (IPAddress)Slice(24, typeof(IPAddress));
            set => Inject(24, value);
        }

        public byte[] ClientHardwareAddress
        {
            get => Slice(28, 16);
            set => Inject(28, value, 16);
        }

        public byte[] ServerName
        {
            get => Slice(44, 64);
            set => Inject(44, value, 64);
        }

        public byte[] BootFilename
        {
            get => Slice(108, 128);
            set => Inject(108, value, 128);
        }

        public byte[] Options
        {
            get => Slice(236, Length - 236);
            set => Inject(236, value, Length - 236);
        }

        public DHCPPacket() : base(236)
        {

        }

        public DHCPPacket(byte[] Data) : base(Data) {}
    }
}
