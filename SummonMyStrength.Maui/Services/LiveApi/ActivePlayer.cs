namespace SummonMyStrength.Maui.Services.LiveApi;

public class ActivePlayer
{
    public Dictionary<string, PlayerAbility> Abilities { get; set; }
    public DetailedStats ChampionStats { get; set; }
    public decimal CurrentGold { get; set; }
    public PlayerRunes FullRunes { get; set; }
    public int Level { get; set; }
    public string SummonerName { get; set; }
    public bool TeamRelativeColors { get; set; }
}
