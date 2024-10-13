namespace SummonMyStrength.Api.Matchmaking;

public enum ReadyCheckState
{
    Invalid,
    InProgress,
    EveryoneReady,
    StrangerNotReady,
    PartyNotReady,
    Error
}
