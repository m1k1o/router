using System.Collections.Generic;
using System.Threading;

namespace Router.Analyzer
{
    class Scenario
    {
        public Interface SourceInterface { get; set; }
        public Interface TargetInterface { get; set; }

        public List<TestCase> TestCases { get; set; } = new List<TestCase>();

        private void Execute(TestCase TestCase)
        {
            ManualResetEvent BlocingWaiting = new ManualResetEvent(false);

            // Callback
            void OnPacketArrival(Handler Handler)
            {
                if (TestCase.Analyze(Handler))
                {
                    // Test is successfull
                    BlocingWaiting.Set();

                    // Save results
                }
            }

            // Register callback
            SourceInterface.OnPacketArrival += OnPacketArrival;

            // Generate packet.
            SourceInterface.SendPacket(TestCase.Generate());

            // Reset Waiting,
            BlocingWaiting.Reset();

            // Wait for response.
            if(BlocingWaiting.WaitOne(TestCase.Timeout))
            {
                // Timeout expired
            }

            // Unregister callback.
            SourceInterface.OnPacketArrival -= OnPacketArrival;
        }

        public void Execute()
        {
            foreach (var TestCase in TestCases)
            {
                Execute(TestCase);
            }
        }
    }
}
