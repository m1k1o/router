using Router.Analyzer;
using System;

namespace Router.Controllers.Analyzer
{
    class PutTestCase : Controller, Executable
    {
        public int? Index { get; set; }

        public TestCase TestCase { get; set; }

        public void Execute()
        {
            if (TestCase == null)
            {
                throw new Exception("Expected TestCase.");
            }

            if (Index == null)
            {
                Index = TestCaseStorage.Insert(TestCase);
            }
            else
            {
                TestCaseStorage.Edit((int)Index, TestCase);
            }
        }

        public object Export() => new
        {
            Index,
            TestCase
        };
    }
}
