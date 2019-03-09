using Router.Analyzer;

namespace Router.Controllers.Analyzer
{
    class GetTestCase : Controller
    {
        public int? Index { get; set; }

        public object Export() => TestCaseStorage.Get((int)Index);
    }
}
