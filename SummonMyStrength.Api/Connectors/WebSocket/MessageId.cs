namespace SummonMyStrength.Api.Connectors.WebSocket;

public enum MessageId
{
    [MessagePath("/lol-gameflow/v1/gameflow-phase")]
    GameflowPhase,

    [MessagePath("/lol-honor-v2/v1/ballot")]
    HonorBallot,

    [MessagePath("/lol-perks/v1/pages")]
    RunePages,

    [MessagePath("/lol-perks/v1/pages/", StartsWith = true)]
    RunePage,

    [MessagePath("/lol-matchmaking/v1/ready-check")]
    ReadyCheck,

    [MessagePath("/lol-end-of-game/v1/eog-stats-block")]
    EndOfGameStats,

    [MessagePath("/lol-pre-end-of-game/v1/currentSequenceEvent")]
    PreEndOfGameSequenceEvent,

    [MessagePath("/lol-champ-select/v1/session")]
    ChampSelectSession,

    [MessagePath("/lol-chat/v1/player-mutes")]
    PlayerMutes,

    UnregisteredMessageId
}
