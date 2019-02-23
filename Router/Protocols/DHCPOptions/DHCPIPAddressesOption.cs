using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPIPAddressesOption : DHCPOption
    {
        public List<IPAddress> IPAddresses { get; private set; }

        public DHCPIPAddressesOption(DHCPOptionCode DHCPOptionCode, byte[] Bytes) : base(DHCPOptionCode)
        {
            IPAddresses = new List<IPAddress>();

            var Length = Bytes.Length;
            for (var i = 0; i < Length; i += 4)
            {
                Add(new IPAddress(BitConverter.ToUInt32(Bytes, i)));
            }
        }

        public DHCPIPAddressesOption(DHCPOptionCode DHCPOptionCode, List<IPAddress> IPAddresses) : base(DHCPOptionCode)
        {
            this.IPAddresses = IPAddresses;
        }

        public DHCPIPAddressesOption(DHCPOptionCode DHCPOptionCode) : base(DHCPOptionCode)
        {
            IPAddresses = new List<IPAddress>();
        }

        public void Add(IPAddress IPAddress)
        {
            IPAddresses.Add(IPAddress);
        }

        public void Remove(IPAddress IPAddress)
        {
            IPAddresses.Remove(IPAddress);
        }

        public override byte[] Bytes
        {
            get
            {
                var ms = new MemoryStream();
                foreach (var IPAddress in IPAddresses)
                {
                    var Bytes = IPAddress.GetAddressBytes();
                    ms.Write(Bytes, 0, Bytes.Length);
                }

                return ms.ToArray();
            }
        }
        public override void Parse(string String)
        {
            IPAddresses = new List<IPAddress>();

            var Entries = String.Split(',');
            foreach (var Entry in Entries)
            {
                Add(IPAddress.Parse(Entry));
            }
        }
    }
}
