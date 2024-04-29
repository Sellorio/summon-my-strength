using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Api.Perks;
using System.Text.Json;

namespace SummonMyStrength.Maui
{
    static class DataStore
    {
        private static readonly string _dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Summon My Strength", "data.json");

        private static readonly DataStoreObject _data;

        public static int? WindowWidth
        {
            get => _data.WindowWidth;
            set => _data.WindowWidth = value;
        }

        public static int? WindowHeight
        {
            get => _data.WindowHeight;
            set => _data.WindowHeight = value;
        }

        public static int? WindowX
        {
            get => _data.WindowX;
            set => _data.WindowX = value;
        }

        public static int? WindowY
        {
            get => _data.WindowY;
            set => _data.WindowY = value;
        }

        public static Dictionary<int, PerkPage[]> RunePages => _data.RunePages;

        public static List<int> PreferredAramChampions
        {
            get => _data.PreferredAramChampions;
            set => _data.PreferredAramChampions = value;
        }

        public static bool AutoAcceptReadyChecks
        {
            get => _data.AutoAcceptReadyChecks;
            set => _data.AutoAcceptReadyChecks = value;
        }

        public static bool HandsFreeMode
        {
            get => _data.HandsFreeMode;
            set => _data.HandsFreeMode = value;
        }

        public static Dictionary<ChampSelectAssignedPosition, List<RecentPick>> RecentPicks => _data.RecentPicks;

        public static Dictionary<int, List<RecentPick>> RecentBansByPick => _data.RecentBansByPick;

        public static Dictionary<ChampSelectAssignedPosition, List<RecentPick>> RecentBansByRole => _data.RecentBansByPosition;

        public static Dictionary<ChampSelectAssignedPosition, List<HandsFreePreference>> HandsFreePreferences => _data.HandsFreePreferences;

        static DataStore()
        {
            _data =
                File.Exists(_dataPath)
                    ? JsonSerializer.Deserialize<DataStoreObject>(File.ReadAllText(_dataPath))
                    : new DataStoreObject();

            _data.RunePages ??= [];
            _data.PreferredAramChampions ??= [];
            _data.RecentPicks ??= [];
            _data.RecentBansByPick ??= [];
            _data.RecentBansByPosition ??= [];
            _data.HandsFreePreferences ??= new Dictionary<ChampSelectAssignedPosition, List<HandsFreePreference>>
            {
                { ChampSelectAssignedPosition.Top, [] },
                { ChampSelectAssignedPosition.Jungle, [] },
                { ChampSelectAssignedPosition.Middle, [] },
                { ChampSelectAssignedPosition.Bottom, [] },
                { ChampSelectAssignedPosition.Support, [] }
            };
        }

        public static async Task SaveAsync()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_dataPath));
            await File.WriteAllTextAsync(_dataPath, JsonSerializer.Serialize(_data));
        }

        public class RecentPick
        {
            public int ChampionKey { get; set; }
            public DateTime PickedAt { get; set; }
        }

        public class HandsFreePreference
        {
            public int PickChampionId { get; set; }
            public List<int> BanChampionIds { get; set; }
            public int SummonerSpell1 { get; set; }
            public int SummonerSpell2 { get; set; }
        }

        private class DataStoreObject
        {
            public int? WindowWidth { get; set; }
            public int? WindowHeight { get; set; }
            public int? WindowX { get; set; }
            public int? WindowY { get; set; }
            public Dictionary<int, PerkPage[]> RunePages { get; set; }
            public List<int> PreferredAramChampions { get; set; }
            public bool AutoAcceptReadyChecks { get; set; }
            public bool HandsFreeMode { get; set; }
            public Dictionary<ChampSelectAssignedPosition, List<RecentPick>> RecentPicks { get; set; }
            public Dictionary<int, List<RecentPick>> RecentBansByPick { get; set; }
            public Dictionary<ChampSelectAssignedPosition, List<RecentPick>> RecentBansByPosition { get; set; }
            public Dictionary<ChampSelectAssignedPosition, List<HandsFreePreference>> HandsFreePreferences { get; set; }
        }
    }
}
