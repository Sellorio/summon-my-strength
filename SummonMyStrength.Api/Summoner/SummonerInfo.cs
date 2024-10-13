namespace SummonMyStrength.Api.Summoner;

public class SummonerInfo
{
    public long AccountId { get; set; }
    public string DisplayName { get; set; }
    public string InternalName { get; set; }
    public bool NameChangeFlag { get; set; }
    public int PercentCompleteForNextLevel { get; set; }
    public ProfilePrivacySetting Privacy { get; set; }
    public int ProfileIconId { get; set; }
    public string Puuid { get; set; }
    public RerollPoints RerollPoints { get; set; }
    public long SummonerId { get; set; }
    public long SummonerLevel { get; set; }
    public bool Unnamed { get; set; }
    public long XpSinceLastLevel { get; set; }
    public long XpUntilNextLevel { get; set; }
}
