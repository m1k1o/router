using Router.Analyzer;

namespace Router.Controllers.Analyzer
{
    class ExportTestCases : Controller
    {
        public object Export() => TestCaseStorage.Export();
    }
}
