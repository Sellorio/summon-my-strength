namespace SummonMyStrength.Api.Matchmaking;

public class ReadyCheck
{
    public long[] DeclinerIds { get; set; }
    public ReadyCheckDodgeWarning DodgeWarning { get; set; }
    public ReadyCheckPlayerResponse PlayerResponse { get; set; }
    public ReadyCheckState State { get; set; }
    public bool SuppressUx { get; set; }
    public float Timer { get; set; }
}
