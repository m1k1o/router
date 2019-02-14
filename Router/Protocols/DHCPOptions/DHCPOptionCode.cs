namespace Router.Protocols.DHCPOptions
{
    enum DHCPOptionCode : byte
    {
        /// <summary>
        /// RFC 2132.
        /// The pad option can be used to cause subsequent fields to align on
        /// word boundaries.
        /// </summary>
        Pad = 0,

        /// <summary>
        /// RFC 2132.
        /// The subnet mask option specifies the client's subnet mask as per RFC
        /// 950 [5].
        /// If both the subnet mask and the router option are specified in a DHCP
        /// reply, the subnet mask option MUST be first.
        /// </summary>
        SubnetMask = 1,

        /// <summary>
        /// RFC 2132.
        /// The router option specifies a list of IP addresses for routers on the
        /// client's subnet. Routers SHOULD be listed in order of preference.
        /// </summary>
        Router = 3,

        /// <summary>
        /// RFC 2132.
        /// The domain name server option specifies a list of Domain Name System
        /// (STD 13, RFC 1035 [8]) name servers available to the client. Servers
        /// SHOULD be listed in order of preference.
        /// </summary>
        DomainNameServer = 6,

        /// <summary>
        /// RFC 2132.
        /// This option is used in a client request (DHCPDISCOVER) to allow the
        /// client to request that a particular IP address be assigned.
        /// </summary>
        RequestedIPAddress = 50,

        /// <summary>
        /// RFC 2132.
        /// This option is used in a client request (DHCPDISCOVER or DHCPREQUEST)
        /// to allow the client to request a lease time for the IP address. In a
        /// server reply(DHCPOFFER), a DHCP server uses this option to specify
        /// the lease time it is willing to offer.
        /// </summary>
        IPAddressLeaseTime = 51,

        /// <summary>
        /// RFC 2132.
        /// This option is used to indicate that the DHCP 'sname' or 'file'
        /// fields are being overloaded by using them to carry DHCP options.A
        /// DHCP server inserts this option if the returned parameters will
        /// exceed the usual space allotted for options.
        /// If this option is present, the client interprets the specified
        /// additional fields after it concludes interpretation of the standard
        /// option fields.
        /// </summary>
        OptionOverload = 52,

        /// <summary>
        /// RFC 2132.
        /// This option is used to convey the type of the DHCP message.
        /// </summary>
        MessageType = 53,

        /// <summary>
        /// RFC 2132.
        /// This option is used in DHCPOFFER and DHCPREQUEST messages, and may
        /// optionally be included in the DHCPACK and DHCPNAK messages.DHCP
        /// servers include this option in the DHCPOFFER in order to allow the
        /// client to distinguish between lease offers.  DHCP clients use the
        /// contents of the 'server identifier' field as the destination address
        /// for any DHCP messages unicast to the DHCP server.  DHCP clients also
        /// indicate which of several lease offers is being accepted by including
        /// this option in a DHCPREQUEST message.
        /// </summary>
        ServerIdentifier = 54,

        /// <summary>
        /// RFC 2132.
        /// This option is used by a DHCP client to request values for specified
        /// configuration parameters.The list of requested parameters is
        /// specified as n octets, where each octet is a valid DHCP option code
        /// as defined in this document.
        /// The client MAY list the options in order of preference.The DHCP
        /// server is not required to return the options in the requested order,
        /// but MUST try to insert the requested options in the order requested
        /// by the client.
        /// </summary>
        ParameterRequestList = 55,

        /// <summary>
        /// RFC 2132.
        /// The end option marks the end of valid information in the vendor field.
        /// </summary>
        End = 255,
    }
}
