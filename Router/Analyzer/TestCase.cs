using System;

namespace Router.Analyzer
{
    class TestCase
    {
        public readonly TimeSpan Timeout = TimeSpan.FromSeconds(25);

        public TestResult Result { get; private set; }

        public byte[] Generate()
        {
            // Generate test packet.
            return new byte[0];
        }

        public bool Analyze(Handler Handler)
        {
            // Analyze received packet.
            return false;
        }
    }
}
