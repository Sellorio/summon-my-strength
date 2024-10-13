namespace SummonMyStrength.Api.PostGame.Honors;

public class HonorEligibleChampion
{
    public string Puuid { get; set; }
    public long SummonerId { get; set; }
    public string SummonerName { get; set; }
    public string ChampionName { get; set; }
    public string SkinSplashPath { get; set; }
    public string Role { get; set; }
}
