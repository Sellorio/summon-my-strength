namespace SummonMyStrength.Api.ChampSelect;

public class ChampSelectTimer
{
    public long AdjustedTimeLeftInPhase { get; set; }
    public long InternalNowInEpochMs { get; set; }
    public bool IsInfinite { get; set; }
    public string Phase { get; set; }
    public long TotalTimeInPhase { get; set; }
}
