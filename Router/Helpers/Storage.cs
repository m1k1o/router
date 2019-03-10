using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Router.Helpers
{
    class Storage<TValue>
    {
        private Dictionary<int, TValue> Instances = new Dictionary<int, TValue>();
        private int Index = 0;

        private string FileName = null;

        public Storage(string FileName = null)
        {
            this.FileName = FileName;
            Load();
        }

        protected void Save()
        {
            if (FileName == null)
            {
                return;
            }

            try
            {
                // Serialize TestCases
                var JSONString = JSON.SerializeObject(Export());

                // Save to File
                File.WriteAllText(FileName, JSONString, Encoding.UTF8);
            }
            catch { }
        }

        protected void Load()
        {
            if (FileName == null)
            {
                return;
            }

            try
            {
                // Get JSON String
                var JsonString = File.ReadAllText(FileName, Encoding.UTF8);

                // Parse
                var TestCases = new List<TValue>();
                JSON.PopulateObject(JsonString, TestCases);

                // Import
                foreach (var TestCase in TestCases)
                {
                    Instances.Add(Index++, TestCase);
                }
            }
            catch { }
        }

        public int Insert(TValue Value)
        {
            Instances.Add(Index, Value);
            Save();
            return Index++;
        }

        public void Edit(int Index, TValue Value)
        {
            Instances[Index] = Value;
            Save();
        }

        public void Remove(int Index)
        {
            Instances.Remove(Index);
            Save();
        }

        public TValue Get(int Index)
        {
            return Instances[Index];
        }

        public Dictionary<int, TValue> Import(List<TValue> List)
        {
            var NewEntries = new Dictionary<int, TValue>();
            foreach (var Value in List)
            {
                NewEntries.Add(Index, Value);
                Instances.Add(Index++, Value);
            }

            Save();
            return NewEntries;
        }

        public List<TValue> Export()
        {
            return Instances.Values.ToList();
        }

        public Dictionary<int, TValue> GetAll()
        {
            return Instances;
        }
    }
}
