using SummonMyStrength.Api.ItemSets;
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

        public static Dictionary<int, PerkPage[]> RunePages => _data.RunePages;

        public static ItemSetList ItemSets
        {
            get => _data.ItemSets;
            set => _data.ItemSets = value;
        }

        public static List<int> PreferredAramChampions
        {
            get => _data.PreferredAramChampions;
            set => _data.PreferredAramChampions = value;
        }

        public static Dictionary<int, int> SelectedSkins => _data.SelectedSkins;

        static DataStore()
        {
            _data =
                File.Exists("data.json")
                    ? JsonSerializer.Deserialize<DataStoreObject>(File.ReadAllText("data.json"))
                    : new DataStoreObject();

            _data.RunePages ??= new Dictionary<int, PerkPage[]>();
            _data.PreferredAramChampions ??= new List<int>();
            _data.SelectedSkins ??= new Dictionary<int, int>();
        }

        public static async Task SaveAsync()
        {
            await File.WriteAllTextAsync("data.json", JsonSerializer.Serialize(_data));
        }

        private class DataStoreObject
        {
            public Dictionary<int, PerkPage[]> RunePages { get; set; }
            public ItemSetList ItemSets { get; set; }
            public List<int> PreferredAramChampions { get; set; }
            public Dictionary<int, int> SelectedSkins { get; set; }
        }
    }
}
