using Router.Protocols.DHCPOptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Router.Protocols
{
    class DHCPOptionCollection : Dictionary<DHCPOptionCode, DHCPOption>
    {
        public DHCPOptionCollection() { }

        public DHCPOptionCollection(byte[] RawData)
        {
            Parse(RawData);
        }

        public DHCPMessageType MessageType { get; private set; } = 0;

        public void Add(DHCPOption DHCPOption)
        {
            Add(DHCPOption.Type, DHCPOption);
        }

        private void Parse(byte[] Bytes)
        {
            var offset = 0;

            do
            {
                var Type = Bytes[offset++];

                if (Type == (byte)DHCPOptionCode.Pad)
                {
                    continue;
                }

                if (Type == (byte)DHCPOptionCode.End)
                {
                    break;
                }

                var Length = Bytes[offset++];

                var Value = new Byte[Length];
                for (var i = 0; i < Length; i++)
                {
                    Value[i] = Bytes[offset++];
                }

                // Shortcut
                if (Type == (byte)DHCPOptionCode.MessageType)
                {
                    MessageType = (DHCPMessageType)Value[0];
                }

                Add((DHCPOptionCode)Type, DHCPOption.Factory(Type, Value));
            } while (offset < Bytes.Length);
        }

        public byte[] Bytes
        {
            get
            {
                var ms = new MemoryStream();
                foreach (var Option in this)
                {
                    if (Option.Key != DHCPOptionCode.End && Option.Key != DHCPOptionCode.Pad)
                    {
                        ms.Write(new byte[] { (byte)Option.Key, (byte)Option.Value.Bytes.Length }, 0, 2);
                    }

                    ms.Write(Option.Value.Bytes, 0, Option.Value.Bytes.Length);
                }

                return ms.ToArray();
            }
        }

        public int Length => Bytes.Length;
    }
}
