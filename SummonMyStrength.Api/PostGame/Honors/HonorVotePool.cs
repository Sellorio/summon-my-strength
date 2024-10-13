namespace SummonMyStrength.Api.PostGame.Honors;

public class HonorVotePool
{
    public int Votes { get; set; }
    public int FromGamePlayed { get; set; }
    public int FromHighHonor { get; set; }
    public int FromRecentHonors { get; set; }
    public int FromRollover { get; set; }
}
