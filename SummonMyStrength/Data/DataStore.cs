using SummonMyStrength.Api.Perks;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Data
{
    static class DataStore
    {
        private static readonly DataStoreObject _data;

        public static Dictionary<int, List<PerkPage>> RunePages => _data.RunePages;

        static DataStore()
        {
            _data =
                File.Exists("data.json")
                    ? JsonSerializer.Deserialize<DataStoreObject>(File.ReadAllText("data.json"))
                    : new DataStoreObject
                    {
                        RunePages = new Dictionary<int, List<PerkPage>>()
                    };
        }

        public static async Task SaveAsync()
        {
            await File.WriteAllTextAsync("data.json", JsonSerializer.Serialize(_data));
        }

        private class DataStoreObject
        {
            public Dictionary<int, List<PerkPage>> RunePages { get; set; }
        }
    }
}
