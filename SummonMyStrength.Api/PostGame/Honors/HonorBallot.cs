namespace SummonMyStrength.Api.PostGame.Honors;

public class HonorBallot
{
    public HonorEligibleChampion[] EligibleAllies { get; set; }
    public HonorEligibleChampion[] EligibleOpponents { get; set; }
    public HonorVotePool VotePool { get; set; }
    public long GameId { get; set; }
    public HonoredPlayers[] HonoredPlayers { get; set; }
}
