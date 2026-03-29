using System.Text.Json.Serialization;
using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Api.PowerSystems.Runes;
using SummonMyStrength.Maui.Services.ChampSelect;

namespace SummonMyStrength.Maui.Data;

public class UserSettings
{
    public int? WindowWidth { get; set; }
    public int? WindowHeight { get; set; }
    public int? WindowX { get; set; }
    public int? WindowY { get; set; }

    // Ready Checks
    public bool AutoAcceptReadyChecks { get; set; }

    // Champ Select
    public TradeResponse PickOrderTradeResponse { get; set; } // TODO

    public bool AutoTradeForPreferredAramChampions { get; set; } = true;
    public List<AramChampionPreferenceGroup> PreferredAramChampionGroups { get; set; } = [];

    [Obsolete("Use PreferredAramChampionGroups instead")]
    [JsonPropertyName("PreferredAramChampions")]
    public List<int> LegacyPreferredAramChampions
    {
        set
        {
            if (PreferredAramChampionGroups.Count > 0 || value == null)
            {
                return;
            }

            PreferredAramChampionGroups =
                value
                    .Select(championId => new AramChampionPreferenceGroup { ChampionIds = [championId] })
                    .ToList();
        }
    }

    public bool AutoPickBanChampions { get; set; }
    public Dictionary<ChampSelectAssignedPosition, List<ChampionPreference>> ChampionPreferences { get; set; } = new()
    {
        { ChampSelectAssignedPosition.Top, new() },
        { ChampSelectAssignedPosition.Jungle, new() },
        { ChampSelectAssignedPosition.Middle, new() },
        { ChampSelectAssignedPosition.Bottom, new() },
        { ChampSelectAssignedPosition.Support, new() }
    };

    // Power Systems
    public Dictionary<int, RunePage[]> RunePages { get; set; } = [];

    public bool AutoSetSummonerSpells { get; set; }
    public Dictionary<int, Dictionary<AssignedPositionOrDefault, SummonerSpellPreferences>> SummonerSpellPreferences { get; set; } = [];

    // Post Game
    public bool SkipPostGameStatsScreen { get; set; }
    public List<string> PostGameGraphStatIds { get; set; } = ["DMG", "KDA", "CS"];

    public bool AutoHonorPlayers { get; set; }
    public bool AlwaysHonorFriends { get; set; } = true;

    public void NormalizeAramChampionPreferenceGroups()
    {
        PreferredAramChampionGroups ??= [];

        HashSet<int> seenChampionIds = [];
        PreferredAramChampionGroups =
            PreferredAramChampionGroups
                .Where(group => group != null)
                .Select(group => new AramChampionPreferenceGroup
                {
                    ChampionIds = (group.ChampionIds ?? [])
                        .Where(seenChampionIds.Add)
                        .ToList()
                })
                .ToList();
    }
}
