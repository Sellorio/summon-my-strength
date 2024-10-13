using SummonMyStrength.Api;
using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.ChampSelect;

namespace SummonMyStrength.Maui.Services;

public class PickBanService : IPickBanService
{
    private readonly LeagueClient _leagueClient;
    private readonly IChampSelectSessionAccessor _champSelectSessionAccessor;

    private int? _bannedChampionId;

    public ChampSelectAssignedPosition? Position { get; private set; }

    public event Func<Task> AssignedPositionChanged;

    public PickBanService(LeagueClient leagueClient, IChampSelectSessionAccessor champSelectSessionAccessor)
    {
        _leagueClient = leagueClient;
        _champSelectSessionAccessor = champSelectSessionAccessor;

        _champSelectSessionAccessor.SessionChanged += async (from, to) =>
        {
            if (to == null)
            {
                await SavePicksAsync(from);
                _bannedChampionId = null;
                Position = null;
                await AssignedPositionChanged.InvokeAsync();
                return;
            }
            
            Position ??= to.Player.Position;

            var banAction =
                _bannedChampionId == null
                    ? to.Actions.SelectMany(x => x).FirstOrDefault(x => x.ActorCellId == to.LocalPlayerCellId && x.Type == ActionType.Ban && !x.IsInProgress.Value)
                    : null;

            if (banAction != null)
            {
                _bannedChampionId = banAction.ChampionId;
            }
        };
    }

    public async Task BanChampionAsync(Champion champion, bool lockIn = true)
    {
        var action = _champSelectSessionAccessor.Session.Actions.SelectMany(x => x).FirstOrDefault(x => x.Type == ActionType.Ban && x.ActorCellId == _champSelectSessionAccessor.Session.LocalPlayerCellId);

        if (action == null)
        {
            return;
        }

        await _leagueClient.ChampSelect.PatchActionAsync(action.Id.Value, new ChampSelectAction { ChampionId = champion.Id, Completed = lockIn });
    }

    public async Task<List<Champion>> GetRecentBansAsync()
    {
        var champions = await _leagueClient.Champions.GetChampionsAsync();

        List<Champion> result = null;

        if (result == null
            && _champSelectSessionAccessor.Session.Player.ChampionId != default
            && DataStore.RecentBansByPick.TryGetValue(_champSelectSessionAccessor.Session.Player.ChampionId, out var picks))
        {
            result = picks.Select(x => champions.First(y => y.Id == x.ChampionKey)).ToList();
        }
        
        if (result == null
            && _champSelectSessionAccessor.Session.Player.ChampionPickIntent != default
            && DataStore.RecentBansByPick.TryGetValue(_champSelectSessionAccessor.Session.Player.ChampionPickIntent, out picks))
        {
            result = picks.Select(x => champions.First(y => y.Id == x.ChampionKey)).ToList();
        }

        if (result == null
            && (Position ?? _champSelectSessionAccessor.Session.Player.Position) != null
            && DataStore.RecentBansByRole.TryGetValue(Position ?? _champSelectSessionAccessor.Session.Player.Position.Value, out picks))
        {
            result = picks.Select(x => champions.First(y => y.Id == x.ChampionKey)).ToList();
        }

        return result ?? new List<Champion>();
    }

    public async Task<List<Champion>> GetRecentPicksAsync()
    {
        var champions = await _leagueClient.Champions.GetChampionsAsync();

        if ((Position ?? _champSelectSessionAccessor.Session.Player.Position) != null
            && DataStore.RecentPicks.TryGetValue(Position ?? _champSelectSessionAccessor.Session.Player.Position.Value, out var picks))
        {
            return picks.Select(x => champions.First(y => y.Id == x.ChampionKey)).ToList();
        }

        return new List<Champion>();
    }

    public async Task SetAssignedPosition(ChampSelectAssignedPosition position)
    {
        if (Position != position && _champSelectSessionAccessor.Session != null)
        {
            Position = position;
            await AssignedPositionChanged.InvokeAsync();
        }
    }

    public async Task PickChampionAsync(Champion champion, bool lockIn = true)
    {
        var action = _champSelectSessionAccessor.Session.Actions.SelectMany(x => x).FirstOrDefault(x => x.Type == ActionType.Pick && x.ActorCellId == _champSelectSessionAccessor.Session.LocalPlayerCellId);

        if (action == null)
        {
            return;
        }

        await _leagueClient.ChampSelect.PatchActionAsync(action.Id.Value, new ChampSelectAction { ChampionId = champion.Id, Completed = action.IsInProgress == true && lockIn });
    }

    private async Task SavePicksAsync(ChampSelectSession session)
    {
        var position = Position ?? session.Player.Position;
        var pick =
            session.Player.ChampionId == default
                ? session.Player.ChampionPickIntent
                : session.Player.ChampionId;

        if (session.Player.ChampionId > 0 && position != null)
        {
            if (!DataStore.RecentPicks.TryGetValue(position.Value, out var recentPicks))
            {
                recentPicks = new List<DataStore.RecentPick>();
                DataStore.RecentPicks[position.Value] = recentPicks;
            }

            AddOrReplaceRecentPick(recentPicks, session.Player.ChampionId);
        }

        if (_bannedChampionId != null && _bannedChampionId > 0)
        {
            if (position != null)
            {
                if (!DataStore.RecentBansByRole.TryGetValue(position.Value, out var recentPicks))
                {
                    recentPicks = new List<DataStore.RecentPick>();
                    DataStore.RecentBansByRole[position.Value] = recentPicks;
                }

                AddOrReplaceRecentPick(recentPicks, _bannedChampionId.Value);
            }

            if (pick != default)
            {
                if (!DataStore.RecentBansByPick.TryGetValue(pick, out var recentPicks))
                {
                    recentPicks = new List<DataStore.RecentPick>();
                    DataStore.RecentBansByPick[pick] = recentPicks;
                }

                AddOrReplaceRecentPick(recentPicks, _bannedChampionId.Value);
            }
        }

        await DataStore.SaveAsync();
    }

    private static void AddOrReplaceRecentPick(List<DataStore.RecentPick> recentPicks, int championKey)
    {
        var matchingRecentPick = recentPicks.FirstOrDefault(x => x.ChampionKey == championKey);

        if (matchingRecentPick != null)
        {
            matchingRecentPick.PickedAt = DateTime.Now;
        }
        else if (recentPicks.Count == 5)
        {
            var oldestRecentPickTime = recentPicks.Min(x => x.PickedAt);
            var oldestRecentPick = recentPicks.First(x => x.PickedAt == oldestRecentPickTime);
            oldestRecentPick.PickedAt = DateTime.Now;
            oldestRecentPick.ChampionKey = championKey;
        }
        else
        {
            recentPicks.Add(new DataStore.RecentPick { ChampionKey = championKey, PickedAt = DateTime.Now });
        }
    }
}
