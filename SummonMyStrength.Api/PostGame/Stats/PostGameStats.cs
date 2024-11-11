using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.PostGame.Stats;

public class PostGameStats
{
    public long GameId { get; set; }

    [JsonPropertyName("ipEarned")]
    public int BlueEssenceGained { get; set; }

    [JsonPropertyName("ipTotal")]
    public int BlueEssenceTotal { get; set; }

    [JsonPropertyName("gameLength")]
    public int GameLengthSeconds { get; set; }

    public PostGamePlayerStats LocalPlayer { get; set; }

    public PostGameTeamStats[] Teams { get; set; }
}
