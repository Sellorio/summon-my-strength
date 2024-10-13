using SummonMyStrength.Api.ChampSelect;
using System.Text.Json.Serialization;

namespace SummonMyStrength.Maui.Services.LiveApi;

public class Player
{
    public string ChampionName { get; set; }
    public bool IsBot { get; set; }
    public bool IsDead { get; set; }
    public IList<PlayerItem> Items { get; set; }
    public int Level { get; set; }
    [JsonPropertyName("position")]
    public string PositionString { get; set; }
    public string RawChampionName { get; set; }
    public decimal RespawnTimer { get; set; }
    public PlayerRunes Runes { get; set; }
    public PlayerScores Scores { get; set; }
    [JsonPropertyName("skinID")]
    public int SkinId { get; set; }
    public string SummonerName { get; set; }
    public PlayerSpells SummonerSpells { get; set; }
    [JsonPropertyName("team")]
    public string TeamString { get; set; }

    [JsonIgnore]
    public ChampSelectAssignedPosition? Position
    {
        get => PositionString?.ToLower() switch
        {
            null => null,
            "" => null,
            "utility" => ChampSelectAssignedPosition.Support,
            "top" => ChampSelectAssignedPosition.Top,
            "middle" => ChampSelectAssignedPosition.Middle,
            "bottom" => ChampSelectAssignedPosition.Bottom,
            "jungle" => ChampSelectAssignedPosition.Jungle,
            _ => throw new NotSupportedException()
        };
        set => PositionString = value switch
        {
            null => "",
            ChampSelectAssignedPosition.Support => "utility",
            ChampSelectAssignedPosition.Top => "top",
            ChampSelectAssignedPosition.Middle => "middle",
            ChampSelectAssignedPosition.Bottom => "bottom",
            ChampSelectAssignedPosition.Jungle => "jungle",
            _ => throw new NotSupportedException()
        };
    }

    [JsonIgnore]
    public PlayerTeam? Team
    {
        get => TeamString?.ToLower() switch
        {
            null => null,
            "" => null,
            "order" => PlayerTeam.Order,
            "chaos" => PlayerTeam.Chaos,
            _ => throw new NotSupportedException()
        };
        set => TeamString = value switch
        {
            null => "",
            PlayerTeam.Order => "ORDER",
            PlayerTeam.Chaos => "CHAOS",
            _ => throw new NotSupportedException()
        };
    }
}
