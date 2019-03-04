using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Router.Analyzer
{
    class Scenario
    {
        public Interface SourceInterface { get; set; }
        public Interface TargetInterface { get; set; }

        public List<TestCase> TestCases { get; set; } = new List<TestCase>();
        public List<TestResult> TestResults { get; set; } = new List<TestResult>();

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
                    //TestResults.Add(TestCase.Results);
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
                //TestResults.Add(new TestResultTimeout());
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
