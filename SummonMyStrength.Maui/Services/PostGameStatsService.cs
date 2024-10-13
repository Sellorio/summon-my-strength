using SummonMyStrength.Api;
using SummonMyStrength.Api.PostGame;

namespace SummonMyStrength.Maui.Services;

internal class PostGameStatsService : IPostGameStatsService
{
    private readonly LeagueClient _leagueClient;

    public PostGameStats PostGameStats { get; private set; }

    public (string Key, string Name, Func<PostGamePlayerStats, decimal> reader)?[] StatReaders { get; } =
    [
        ("KDA", "KDA", (PostGamePlayerStats x) => (x.Kills + x.Assists) / (decimal)Math.Max(x.Deaths, 1)),
        ("K", "Kills", (PostGamePlayerStats x) => x.Kills),
        ("A", "Assists", (PostGamePlayerStats x) => x.Assists),
        ("D", "Deaths", (PostGamePlayerStats x) => x.Deaths),
        ("CS", "Critter Score (CS)", (PostGamePlayerStats x) => x.CritterScore),
        ("G", "Gold", (PostGamePlayerStats x) => x.Gold),
        ("VS", "Vision Score", (PostGamePlayerStats x) => x.VisionScore),
        ("CC", "Crowd Control Score", (PostGamePlayerStats x) => x.CrowdControlScore),
        ("CCR", "Crowd Control Received", (PostGamePlayerStats x) => x.CrowdControlReceived),
        ("DMG", "Damage", (PostGamePlayerStats x) => x.TotalDamage),
        ("MDMG", "Magic Damage", (PostGamePlayerStats x) => x.MagicDamage),
        ("PDMG", "Physical Damage", (PostGamePlayerStats x) => x.PhysicalDamage),
        ("TMDG", "True Damage", (PostGamePlayerStats x) => x.TrueDamage),
        ("DMGT", "Damage Taken", (PostGamePlayerStats x) => x.DamageTaken),
        ("MDMGT", "Magic Damage Taken", (PostGamePlayerStats x) => x.MagicDamageTaken),
        ("PDMGT", "Physical Damage Taken", (PostGamePlayerStats x) => x.PhysicalDamageTaken),
        ("TDMGT", "True Damage Taken", (PostGamePlayerStats x) => x.TrueDamageTaken),
        ("MIT", "Self Mitigated Damage", (PostGamePlayerStats x) => x.SelfMitigatedDamage),
        ("HEAL", "Total Healing", (PostGamePlayerStats x) => x.Healing),
        ("AHEAL", "Ally Healing", (PostGamePlayerStats x) => x.AllyHealing),
        ("ASHI", "Ally Shielding", (PostGamePlayerStats x) => x.AllyShielding),
    ];

    public event Func<Task> PostGameStatsChanged;

    public PostGameStatsService(LeagueClient leagueClient)
    {
        _leagueClient = leagueClient;
        _leagueClient.PostGame.PostGameStatsPosted += PostGame_PostGameStatsPosted;
    }

    private async Task PostGame_PostGameStatsPosted(PostGameStats stats)
    {
        PostGameStats = stats;

        await PostGameStatsChanged.InvokeAsync();

        if (DataStore.SkipPostGameStatsScreen)
        {
            await _leagueClient.PostGame.DismissStatsAsync();
        }
    }
}
