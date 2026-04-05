using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.ChampSelect.Trades;

public class OngoingTrade
{
    public long Id { get; set; }
    public bool InitiatedByLocalPlayer { get; set; }
    public long OtherSummonerIndex { get; set; }
    public string RequesterChampionName { get; set; }
    public string RequesterChampionSplashPath { get; set; }
    public int RequesterChampionId { get; set; }
    public string ResponderChampionName { get; set; }
    public long ResponderIndex { get; set; }
    public ChampSelectTradeState State { get; set; }
}
