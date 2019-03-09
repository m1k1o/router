using Router.Analyzer;

namespace Router.Controllers.Analyzer
{
    class GetAllTestCases : Controller
    {
        public object Export() => TestCaseStorage.GetAll();
    }
}
