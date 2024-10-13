using System.Linq;
using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.ChampSelect;

public class ChampSelectSession
{
    public ChampSelectAction[][] Actions { get; set; }
    public bool AllowBattleBoost { get; set; }
    public bool AllowDuplicatePicks { get; set; }
    public bool AllowLockedEvents { get; set; }
    public bool AllowRerolling { get; set; }
    public bool AllowSkinSelection { get; set; }
    public CampSelectBannedChampions Bans { get; set; }
    public int[] BenchChampionIds { get; set; }
    public bool BenchEnabled { get; set; }
    public int BoostableSkinCount { get; set; }
    public ChampSelectChatRoomDetails ChatDetails { get; set; }
    public int Counter { get; set; }
    public EntitledFeatureState EntitledFeatureState { get; set; }
    public int GameId { get; set; }
    public bool HasSimultaneousBans { get; set; }
    public bool HasSimultaneousPicks { get; set; }
    public bool IsCustomGame { get; set; }
    public bool IsSpectating { get; set; }
    public int LocalPlayerCellId { get; set; }
    public int LockedEventIndex { get; set; }
    public ChampSelectPlayerSelection[] MyTeam { get; set; }
    public int RecoveryCounter { get; set; }
    public int RerollsRemaining { get; set; }
    public bool SkipChampionSelect { get; set; }
    public ChampSelectPlayerSelection[] TheirTeam { get; set; }
    public ChampSelectTimer Timer { get; set; }
    public ChampSelectTradeContract[] Trades { get; set; }
    public ChampSelectTradeContract[] PickOrderSwaps { get; set; }

    private ChampSelectPlayerSelection _player;

    [JsonIgnore]
    public ChampSelectPlayerSelection Player => _player ??= MyTeam.First(x => x.CellId == LocalPlayerCellId);

    [JsonIgnore]
    public bool IsPickingIntent =>
        Timer.Phase == "PLANNING" ||
        !IsBanning && Actions.SelectMany(x => x).Any(x => x.Type == ActionType.Pick && x.ActorCellId == LocalPlayerCellId && !x.IsInProgress.Value && !x.Completed.Value);

    [JsonIgnore]
    public bool IsActivelyPicking => Actions.SelectMany(x => x).Any(x => x.Type == ActionType.Pick && x.ActorCellId == LocalPlayerCellId && x.IsInProgress.Value);

    [JsonIgnore]
    public bool IsBanning => Timer.Phase != "PLANNING" && Actions.SelectMany(x => x).Any(x => x.Type == ActionType.Ban && x.ActorCellId == LocalPlayerCellId && x.IsInProgress.Value && !x.Completed.Value);
}
