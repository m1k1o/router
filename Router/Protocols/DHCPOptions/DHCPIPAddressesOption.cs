using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPIPAddressesOption : DHCPOption
    {
        public List<IPAddress> IPAddresses { get; set; }

        public DHCPIPAddressesOption(DHCPOptionCode DHCPOptionCode) : base(DHCPOptionCode)
        {
            IPAddresses = new List<IPAddress>();
        }

        public DHCPIPAddressesOption(DHCPOptionCode DHCPOptionCode, byte[] Bytes) : base(DHCPOptionCode)
        {
            IPAddresses = new List<IPAddress>();

            var Length = Bytes.Length;
            for (var i = 0; i < Length; i += 4)
            {
                IPAddresses.Add(new IPAddress(BitConverter.ToUInt32(Bytes, i)));
            }
        }

        public DHCPIPAddressesOption(DHCPOptionCode DHCPOptionCode, List<IPAddress> IPAddresses) : base(DHCPOptionCode)
        {
            this.IPAddresses = IPAddresses;
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
    }
}
