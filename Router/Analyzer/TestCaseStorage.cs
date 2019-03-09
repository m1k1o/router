﻿using Router.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Router.Analyzer
{
    static class TestCaseStorage
    {
        const string FILE_NAME = "TestCases.json";

        private static Dictionary<int, TestCase> Instances = new Dictionary<int, TestCase>();
        private static int Index = 0;

        static TestCaseStorage()
        {
            try
            {
                LoadFromFile();
            }
            catch { }
        }

        static void SaveToFile()
        {
            // Serialize TestCases
            var JSONString = JSON.SerializeObject(Export());

            // Save to File
            File.WriteAllText(FILE_NAME, JSONString, Encoding.UTF8);
        }

        static void LoadFromFile()
        {
            // Get JSON String
            var JsonString = File.ReadAllText(FILE_NAME, Encoding.UTF8);

            // Parse & Import
            var TestCases = new List<TestCase>();
            JSON.PopulateObject(JsonString, TestCases);

            foreach (var TestCase in TestCases)
            {
                Instances.Add(Index++, TestCase);
            }
        }

        public static int Insert(TestCase TestCase)
        {
            Instances.Add(Index, TestCase);

            try
            {
                SaveToFile();
            }
            catch { }

            return Index++;
        }

        public static void Remove(int Index)
        {
            Instances.Remove(Index);

            try
            {
                SaveToFile();
            }
            catch { }
        }

        public static TestCase Get(int Index)
        {
            return Instances[Index];
        }

        public static void Import(List<TestCase> List)
        {
            foreach (var TestCase in List)
            {
                Instances.Add(Index++, TestCase);
            }

            try {
                SaveToFile();
            }
            catch { }
        }

        public static List<TestCase> Export()
        {
            return Instances.Values.ToList();
        }

        public static Dictionary<int, TestCase> GetAll()
        {
            return Instances;
        }
    }
}
