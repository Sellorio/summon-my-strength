using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.PostGame.Stats;

public class UpdatedLpInfo
{
    [JsonPropertyName("leaguePointsDelta")]
    public int LpDelta { get; set; }
}
