using Router.Analyzer;

namespace Router.Controllers.Analyzer
{
    class AllTestCases : Controller
    {
        public object Export() => TestCaseStorage.GetAll();
    }
}
