using Router.Analyzer;
using System;

namespace Router.Controllers.Analyzer
{
    class RemoveTestCase : Controller, Executable
    {
        public int? Index { get; set; }

        public void Execute()
        {
            if (Index == null)
            {
                throw new Exception("Expected Index.");
            }

            TestCaseStorage.Remove((int)Index);
        }

        public object Export() => new
        {
            Removed = true
        };
    }
}
