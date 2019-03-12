using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Router.Analyzer
{
    class Scenario : List<TestCase>
    {
        public event Action OnStarted = delegate { };
        public event Action OnStopped = delegate { };
        public event Action<string> OnLogMessage = delegate { };

        [JsonIgnore]
        public Interface GeneratorInterface { get; set; }
        [JsonIgnore]
        public Interface AnalyzerInterface { get; set; }

        public int Total => Count;
        public int Successful { get; private set; }
        public int Percentage => Successful / Total * 100;

        public DateTime TimeStarted { get; private set; }
        public DateTime TimeStopped { get; private set; }

        public TimeSpan TimeEpalsed
        {
            get
            {
                if (Running)
                {
                    return DateTime.Now - TimeStarted;
                }
                else
                {
                    return TimeStopped - TimeStarted;
                }
            }
        }

        public bool Running => TimeStopped == DateTime.MinValue;

        private ManualResetEvent BlocingWaiting = new ManualResetEvent(false);
        private Thread Thread;

        public void RunTestCase(TestCase TestCase)
        {
            TestCase.GeneratorInterface = GeneratorInterface;
            TestCase.AnalyzerInterface = AnalyzerInterface;

            // Register events
            Action OnStopped = null;
            TestCase.OnLogMessage += OnLogMessage;
            TestCase.OnStopped += OnStopped = () =>
            {
                if (TestCase.Status == TestCaseStatus.Success)
                {
                    Successful++;
                }

                // Unregister events
                TestCase.OnLogMessage -= OnLogMessage;
                TestCase.OnStopped -= OnStopped;
                BlocingWaiting.Set();
            };

            // Start test case
            BlocingWaiting.Reset();
            TestCase.Start();
        }

        public void Start()
        {
            TimeStarted = DateTime.Now;
            TimeStopped = DateTime.MinValue;
            Successful = 0;
            OnStarted();

            // Wait for result
            Thread = new Thread(() => {
                foreach (var TestCase in this)
                {
                    RunTestCase(TestCase);

                    // Wait for complete
                    BlocingWaiting.WaitOne();
                }

                TimeStopped = DateTime.Now;
                OnStopped();
            });
            Thread.Start();
        }
     }
}
