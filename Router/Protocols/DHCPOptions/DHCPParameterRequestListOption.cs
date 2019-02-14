﻿using System;
using System.Collections.Generic;

namespace Router.Protocols.DHCPOptions
{
    class DHCPParameterRequestListOption : DHCPOption
    {
        private List<DHCPOptionCode> Codes;

        public DHCPParameterRequestListOption(byte[] Bytes) : base(DHCPOptionCode.ParameterRequestList)
        {
            var Length = Bytes.Length;
            for (var i = 0; i < Length; i += 4)
            {
                Codes.Add((DHCPOptionCode)Bytes[i]);
            }
        }

        public DHCPParameterRequestListOption(List<DHCPOptionCode> Codes) : base(DHCPOptionCode.ParameterRequestList)
        {
            this.Codes = Codes;
        }

        public DHCPParameterRequestListOption() : base(DHCPOptionCode.ParameterRequestList)
        {
            Codes = new List<DHCPOptionCode>();
        }

        public void Add(DHCPOptionCode Code)
        {
            Codes.Add(Code);
        }

        public void Remove(DHCPOptionCode Code)
        {
            Codes.Remove(Code);
        }

        public override byte[] Bytes
        {
            get
            {
                var Array = new Byte[Codes.Count];
                int i = 0;
                foreach (var Code in Codes)
                {
                    Array[i++] = (byte)Code;
                }

                return Array;
            }
        }
    }
}
