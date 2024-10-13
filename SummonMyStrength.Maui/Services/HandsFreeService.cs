using SummonMyStrength.Api;
using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Maui.Services.ChampSelect;
using System.Diagnostics;

namespace SummonMyStrength.Maui.Services;

internal class HandsFreeService(LeagueClient leagueClient, IPickBanService pickBanService, IChampSelectSessionAccessor champSelectSessionAccessor, IChampSelectSessionAbstractor champSelectSessionAbstractor) : IHandsFreeService
{
    private Timer _timer;
    private Champion[] _champions;

    public async Task InitialiseAsync()
    {
        if (_timer != null)
        {
            throw new InvalidOperationException("Already initialized.");
        }

        _timer = new Timer(_ => Task.Run(() => ProcessHandsFreeActionsAsync(false, true)), null, Timeout.Infinite, Timeout.Infinite);
        _champions = await leagueClient.Champions.GetChampionsAsync();

        pickBanService.AssignedPositionChanged += () => ProcessHandsFreeActionsAsync(true, true);
        champSelectSessionAbstractor.OnPhaseChanged += ChampSelectPhaseChanged;
        champSelectSessionAbstractor.OnSelectedChampionIdChanged += SelectedChampionIdChanged;
        champSelectSessionAccessor.SessionChanged += (from, to) => champSelectSessionAbstractor.ApplyChangesAsync(to);
    }

    private async Task ChampSelectPhaseChanged(ChampSelectPhase from, ChampSelectPhase to)
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);

        switch (to)
        {
            case ChampSelectPhase.PickIntent:
                await ProcessHandsFreeActionsAsync(false, false);
                break;
            case ChampSelectPhase.WaitingToBan:
                await ProcessHandsFreeActionsAsync(false, false);
                break;
            case ChampSelectPhase.Banning:
                await ProcessHandsFreeActionsAsync(false, false);
                _timer.Change(5000, 1000);
                break;
            case ChampSelectPhase.WaitingToPick:
                await ProcessHandsFreeActionsAsync(false, false);
                break;
            case ChampSelectPhase.Picking:
                _timer.Change(5000, 1000);
                break;
            default:
                break;
        }
    }

    private async Task SelectedChampionIdChanged(int? from, int? to)
    {
        if (to != null)
        {
            await UpdateSummonerSpellsFromPreferencesAsync(to.Value);
        }
    }

    private async Task ProcessHandsFreeActionsAsync(bool overrideSelectedChampionForPick, bool lockIn)
    {
        if (!DataStore.HandsFreeMode || pickBanService.Position == null)
        {
            return;
        }

        _timer.Change(Timeout.Infinite, Timeout.Infinite);

        var session = champSelectSessionAccessor.Session;

        if (session == null)
        {
            return;
        }

        switch (champSelectSessionAbstractor.Phase)
        {
            case ChampSelectPhase.PickIntent:
                await PickChampionFromPreferencesAsync(session, lockIn);
                break;
            case ChampSelectPhase.WaitingToBan:
            case ChampSelectPhase.Banning:
                await BanChampionFromPreferencesAsync(session, lockIn);
                break;
            case ChampSelectPhase.WaitingToPick:
            case ChampSelectPhase.Picking:
                await PickChampionFromPreferencesAsync(session, lockIn);
                break;
        }
    }

    private async Task BanChampionFromPreferencesAsync(ChampSelectSession session, bool lockIn)
    {
        var banAction = champSelectSessionAccessor.Session.Actions.SelectMany(x => x).FirstOrDefault(x => x.ActorCellId == champSelectSessionAccessor.Session.LocalPlayerCellId && x.Type == ActionType.Ban);

        if (banAction != null && banAction.ChampionId != null && banAction.ChampionId != 0)
        {
            var banChampion = _champions.FirstOrDefault(x => x.Id == banAction.ChampionId);

            if (banChampion != null)
            {
                await pickBanService.BanChampionAsync(banChampion, lockIn);
                return;
            }
        }

        if (champSelectSessionAbstractor.SelectedChampionId == null)
        {
            return;
        }

        var preferences = DataStore.HandsFreePreferences[pickBanService.Position.Value];
        var selectedPreference = preferences.FirstOrDefault(x => x.PickChampionId == champSelectSessionAbstractor.SelectedChampionId);

        if (selectedPreference != null)
        {
            var firstAvailableBan = selectedPreference.BanChampionIds.FirstOrDefault(x => !IsChampionDisbled(session, true, x));

            if (firstAvailableBan != default)
            {
                var banChampion = _champions.FirstOrDefault(x => x.Id == firstAvailableBan);

                if (banChampion != null)
                {
                    await pickBanService.BanChampionAsync(banChampion, lockIn);
                }
            }
        }
    }

    private async Task PickChampionFromPreferencesAsync(ChampSelectSession session, bool lockIn)
    {
        int championId;

        if (champSelectSessionAbstractor.SelectedChampionId != null)
        {
            championId = champSelectSessionAbstractor.SelectedChampionId.Value;
        }
        else
        {
            var preferences = DataStore.HandsFreePreferences[pickBanService.Position.Value];
            var firstAvailablePreference = preferences.FirstOrDefault(x => !IsChampionDisbled(session, false, x.PickChampionId));

            if (firstAvailablePreference == null)
            {
                return;
            }

            championId = firstAvailablePreference.PickChampionId;
        }

        var champion = _champions.FirstOrDefault(x => x.Id == championId);

        if (champion != null)
        {
            await pickBanService.PickChampionAsync(champion, lockIn);
        }
    }

    private async Task UpdateSummonerSpellsFromPreferencesAsync(int championId)
    {
        var preferences = DataStore.HandsFreePreferences[pickBanService.Position.Value];
        var firstAvailablePreference = preferences.FirstOrDefault(x => x.PickChampionId == championId);

        if (firstAvailablePreference != null)
        {
            try
            {
                await leagueClient.ChampSelect.PatchMySelectionAsync(new MySelection
                {
                    Spell1Id = firstAvailablePreference.SummonerSpell1,
                    Spell2Id = firstAvailablePreference.SummonerSpell2
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }
    }

    //private async Task PickRecentChampionAsync()
    //{
    //    var recentPick =
    //        pickBanService.Position != null &&
    //        DataStore.RecentPicks.TryGetValue(pickBanService.Position.Value, out var picks) &&
    //        picks.Any()
    //            ? picks.OrderByDescending(x => x.PickedAt).First()
    //            : null;

    //    if (recentPick != null)
    //    {
    //        var recentPickChampion = _champions.FirstOrDefault(x => x.Id == recentPick.ChampionKey);
    //        await pickBanService.PickChampionAsync(recentPickChampion);
    //    }
    //}

    //private async Task PickRecentBanAsync(ChampSelectSession session, int selectedChampionId)
    //{
    //    var recentBans = Enumerable.Empty<int>();

    //    if (selectedChampionId != default && DataStore.RecentBansByPick.TryGetValue(selectedChampionId, out var bans))
    //    {
    //        recentBans = recentBans.Union(bans.OrderByDescending(x => x.PickedAt).Select(x => x.ChampionKey));
    //    }

    //    if (pickBanService.Position != null && DataStore.RecentBansByRole.TryGetValue(pickBanService.Position.Value, out bans))
    //    {
    //        recentBans = recentBans.Union(bans.OrderByDescending(x => x.PickedAt).Select(x => x.ChampionKey));
    //    }

    //    var championToBanId = recentBans.FirstOrDefault(x => !IsChampionDisbled(session, true, x));

    //    if (championToBanId != default)
    //    {
    //        var banChampion = _champions.FirstOrDefault(x => x.Id == championToBanId);
    //        await pickBanService.BanChampionAsync(banChampion);
    //    }
    //}

    private static bool IsChampionDisbled(ChampSelectSession session, bool isBanning, int championId)
    {
        if (isBanning)
        {
            return
                session.Actions.SelectMany(x => x).Any(x => x.Type == ActionType.Ban && x.ChampionId == championId && x.ActorCellId != session.LocalPlayerCellId) ||
                session.MyTeam.Any(x => x.ChampionPickIntent == championId);

        }
        else
        {
            return
                session.Actions.SelectMany(x => x).Any(x =>
                    x.Type == ActionType.Ban && x.ChampionId == championId ||
                    x.ActorCellId == session.LocalPlayerCellId && x.Type == ActionType.Pick && x.Completed == true) ||
                session.MyTeam.Any(x => x.CellId != session.LocalPlayerCellId && x.ChampionId == championId) ||
                session.TheirTeam.Any(x => x.ChampionId == championId);
        }
    }
}
