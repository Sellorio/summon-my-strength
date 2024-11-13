namespace SummonMyStrength.Api.Social;

public class InGameReportStatus
{
    public bool IsPlayerMuted { get; set; }
    public bool IsSettingsMuted { get; set; }
    public bool IsSystemMuted { get; set; }
    public string ObfuscatedPuuid { get; set; }
    public string Puuid { get; set; }
}
