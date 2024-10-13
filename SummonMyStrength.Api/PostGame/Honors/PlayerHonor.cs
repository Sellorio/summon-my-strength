namespace SummonMyStrength.Api.PostGame.Honors;

public class PlayerHonor
{
    public long SummonerId { get; set; }
    public string Puuid { get; set; }
    public string HonorType { get; set; } = "HEART";
    public long GameId { get; set; }
}
