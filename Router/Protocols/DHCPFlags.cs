namespace Router.Protocols
{
    //                     1 1 1 1 1 1
    // 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    // |B|             MBZ             |
    // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    //
    // B:  BROADCAST flag
    // MBZ:  MUST BE ZERO(reserved for future use)
    public enum DHCPFlags : ushort
    {
        // Response as Unicast.
        Unicast = 0 << 15,

        // Response as Broadcast.
        Broadcast = 1 << 15
    }
}
