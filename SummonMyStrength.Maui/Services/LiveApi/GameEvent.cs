using System.Text.Json.Serialization;

namespace SummonMyStrength.Maui.Services.LiveApi;

public class GameEvent
{
    [JsonPropertyName("EventID")]
    public int Id { get; set; }
    [JsonPropertyName("EventName")]
    public string TypeString { get; set; }
    public int? KillStreak { get; set; }
    public string Acer { get; set; }
    public string AcingTeam { get; set; }

    [JsonIgnore]
    public GameEventType? Type
    {
        get => TypeString?.ToLower() switch
        {
            null => null,
            "" => null,
            "GameStart" => GameEventType.GameStart,
            "MinionsSpawning" => GameEventType.MinionsSpawning,
            "FirstBrick" => GameEventType.FirstBlood,
            "TurretKilled" => GameEventType.TurretKilled,
            "InhibKilled" => GameEventType.InhibitorKilled,
            "DragonKill" => GameEventType.DragonKilled,
            "HeraldKill" => GameEventType.RiftHeraldKilled,
            "BaronKill" => GameEventType.BaronKilled,
            "ChampionKill" => GameEventType.ChampionKilled,
            "MultiKill" => GameEventType.MultiKill,
            "Ace" => GameEventType.Ace,
            _ => throw new NotSupportedException()
        };
        set => TypeString = value switch
        {
            null => "",
            GameEventType.GameStart => "GameStart",
            GameEventType.MinionsSpawning => "MinionsSpawning",
            GameEventType.FirstBlood => "FirstBrick",
            GameEventType.TurretKilled => "TurretKilled",
            GameEventType.InhibitorKilled => "InhibKilled",
            GameEventType.DragonKilled => "DragonKill",
            GameEventType.RiftHeraldKilled => "HeraldKill",
            GameEventType.BaronKilled => "BaronKill",
            GameEventType.ChampionKilled => "ChampionKill",
            GameEventType.MultiKill => "MultiKill",
            GameEventType.Ace => "Ace",
            _ => throw new NotSupportedException()
        };
    }
}
