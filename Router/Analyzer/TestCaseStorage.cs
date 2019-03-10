using Router.Helpers;
using System.Collections.Generic;

namespace Router.Analyzer
{
    static class TestCaseStorage
    {
        private static Storage<TestCase> Storage = new Storage<TestCase>("TestCases.json");

        public static int Insert(TestCase TestCase) => Storage.Insert(TestCase);

        internal static void Edit(int Index, TestCase TestCase) => Storage.Edit(Index, TestCase);

        public static void Remove(int Index) => Storage.Remove(Index);

        public static TestCase Get(int Index) => Storage.Get(Index);

        public static Dictionary<int, TestCase> Import(List<TestCase> List) => Storage.Import(List);

        public static List<TestCase> Export() => Storage.Export();

        public static Dictionary<int, TestCase> GetAll() => Storage.GetAll();
    }
}
