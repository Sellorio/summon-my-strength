namespace SummonMyStrength.Api.ChampSelect
{
    public enum ChampSelectTradeState
    {
        Available,
        Busy,
        Invalid,
        Received,
        Sent,
        Declined,
        Cancelled,
        Accepted,
        NewUnsupportedValue // if riot adds a new value, this will be returned instead of breaking the app
    }
}
