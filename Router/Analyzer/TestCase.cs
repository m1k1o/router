using System;

namespace Router.Analyzer
{
    class TestCase
    {
        public readonly TimeSpan Timeout = TimeSpan.FromSeconds(25);

        public bool Success { get; private set; } = false;

        public byte[] Generate()
        {
            // Generate test packet.
            return new byte[0];
        }

        public bool Analyze(Handler Handler)
        {
            Success = true;
            
            // Analyze received packet.
            return true;
        }
    }
}
