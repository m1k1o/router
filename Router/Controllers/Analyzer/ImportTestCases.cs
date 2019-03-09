using Router.Analyzer;
using System;
using System.Collections.Generic;

namespace Router.Controllers.Analyzer
{
    class ImportTestCase : Controller, Executable
    {
        public List<TestCase> TestCases { get; set; }

        public void Execute()
        {
            if (TestCases == null || TestCases.Count == 0)
            {
                throw new Exception("Expected TestCase.");
            }

            TestCaseStorage.Import(TestCases);
        }

        public object Export() => new
        {
            Success = true,
            AddedTotal = TestCases.Count
        };
    }
}
