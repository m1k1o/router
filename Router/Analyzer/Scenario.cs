using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.Analyzer
{
    class Scenario
    {
        public Interface SourceInterface { get; set; }
        public Interface TargetInterface { get; set; }

        public List<TestCase> TestCases { get; set; } = new List<TestCase>();

        private TestCase CurrentTestCase = null;

        public void Execute()
        {
            SourceInterface.OnPacketArrival += OnPacketArrival;

            // While:
                // Generate packets.

                // Wait Timeout.

                // Analyze otuputs.
                
            SourceInterface.OnPacketArrival -= OnPacketArrival;
            
            // Analyze results
        }

        public void OnPacketArrival(Handler Handler)
        {
            //CurrentTestCase.Analyze(Handler);
            Console.WriteLine("Target received Packet.");
        }
    }
}
