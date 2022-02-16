namespace SummonMyStrength.Api.Gameflow
{
    public enum GameflowPhase
    {
        None,
        Lobby,
        Matchmaking,
        CheckedIntoTournament,
        ReadyCheck,
        ChampSelect,
        GameStart,
        FailedToLaunch,
        InProgress,
        Reconnect,
        WaitingForStats,
        PreEndOfGame,
        EndOfGame,
        TerminatedInError,
        NewUnsupportedValue // if riot adds a new value, this will be returned instead of breaking the app
    }
}
