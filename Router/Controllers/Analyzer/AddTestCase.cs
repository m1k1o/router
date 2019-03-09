using Router.Analyzer;
using System;

namespace Router.Controllers.Analyzer
{
    class AddTestCase : Controller, Executable
    {
        private int Index;

        public TestCase TestCase { get; set; }

        public void Execute()
        {
            if (TestCase == null)
            {
                throw new Exception("Expected TestCase.");
            }

            Index = TestCaseStorage.Insert(TestCase);
        }

        public object Export() => new
        {
            Index
        };
    }
}
