using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.Analyzer
{
    class TestCase
    {
        // LOG

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
