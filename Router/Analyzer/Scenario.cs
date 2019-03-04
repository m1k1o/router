using System;
using System.Collections.Generic;

namespace Router.Analyzer
{
    class Scenario : List<TestCase>
    {
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
                } else
                {
                    return TimeStopped - TimeStarted;
                }
            }
        }

        public bool Running => TimeStopped == DateTime.MinValue;
        public string Log { get; private set; }

        public void Execute(Interface GeneratorInterface, Interface AnalyzerInterface)
        {
            TimeStarted = DateTime.Now;
            TimeStopped = DateTime.MinValue;
            Successful = 0;

            foreach (var TestCase in this)
            {
                TestCase.Execute(GeneratorInterface, AnalyzerInterface);
                Log += TestCase.Log;

                if (TestCase.Success) Successful++;
            }

            TimeStopped = DateTime.Now;
        }
    }
}
