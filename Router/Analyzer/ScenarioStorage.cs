using Router.Helpers;
using System.Collections.Generic;

namespace Router.Analyzer
{
    static class ScenarioStorage
    {
        private static Storage<Scenario> Storage = new Storage<Scenario>("Scenarios.json");

        public static int Insert(Scenario Scenario) => Storage.Insert(Scenario);

        internal static void Edit(int Index, Scenario Scenario) => Storage.Edit(Index, Scenario);

        public static void Remove(int Index) => Storage.Remove(Index);

        public static Scenario Get(int Index) => Storage.Get(Index);

        public static Dictionary<int, Scenario> Import(List<Scenario> List) => Storage.Import(List);

        public static List<Scenario> Export() => Storage.Export();

        public static Dictionary<int, Scenario> GetAll() => Storage.GetAll();
    }
}
