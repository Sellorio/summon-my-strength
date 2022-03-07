using SEA.Mvvm.ModelSupport;
using SummonMyStrength.Api;
using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Api.Gameflow;
using SummonMyStrength.Api.ItemSets;
using SummonMyStrength.Api.Perks;
using SummonMyStrength.Api.Summoner;
using SummonMyStrength.Data;
using SummonMyStrength.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SummonMyStrength.Models
{
    class MainModel : ModelBase<MainViewModel>
    {
        private readonly List<(long CellId, int ChampionId)> _attemptedTrades = new List<(long CellId, int ChampionId)>();
        private int? _skinSetForChampionId;
        private int _currentChampionBaseSkin;

        private bool _ignoreNextItemSetUpdate;

        public MainModel(IInterface @interface, MainViewModel viewModel)
            : base(@interface, viewModel)
        {
        }

        protected override void BindModel(ModelBinder<MainViewModel> modelBinder)
        {
            modelBinder.BindAsync(x => x.Initialise, InitialiseAsync);
            modelBinder.Bind(x => x.OpenApiPlayground, OpenApiPlayground);
            modelBinder.BindAsync(x => x.ReApplyChampionRunes, ApplyChampionRunesAsync);
            modelBinder.BindAsync(x => x.SaveRunes, SaveRunePagesAsync);
            modelBinder.Bind(x => x.EditAramPreferences, EditAramPreferences);

            modelBinder.BindChange(x => x.RuneChampionContext, () => ViewModel.ReApplyChampionRunes.Execute(null));
        }
        
        private async Task InitialiseAsync()
        {
            App.LeagueClient = new LeagueClient();
            App.LeagueClient.Connected += ConnectedAsync;
            App.LeagueClient.Gameflow.GameflowPhaseChanged += GameflowPhaseChangedAsync;
            App.LeagueClient.ChampSelect.SessionChanged += ChampSelectSessionChanged;
            App.LeagueClient.ItemSets.ItemSetsChanged += ItemSetsChangedAsync;

            await App.LeagueClient.ConnectAsync();
        }

        private async Task ConnectedAsync()
        {
            if (ViewModel.Champions == null)
            {
                ViewModel.Champions = await App.LeagueClient.Champions.GetChampionsAsync();
            }

            await GameflowPhaseChangedAsync(await App.LeagueClient.Gameflow.GetGameflowPhaseAsync());

            if (DataStore.ItemSets == null)
            {
                ViewModel.ItemSetsMessage = "Save an item set in game to begin sync.";
            }
            else
            {
                ViewModel.ItemSetsMessage = "Syncing item sets...";

                SummonerInfo summonerInfo = null;

                // try to fetch the summoner info for 10 seconds, give up if the info still isn't loaded
                for (var i = 0; i < 20; i++)
                {
                    try
                    {
                        summonerInfo = await App.LeagueClient.Summoner.GetCurrentSummonerAsync();
                        break;
                    }
                    catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                    {
                        await Task.Delay(500);
                    }
                    catch
                    {
                        break;
                    }
                }

                if (summonerInfo == null)
                {
                    ViewModel.ItemSetsMessage = "Failed to sync item sets.";
                    return;
                }

                DataStore.ItemSets.AccountId = summonerInfo.AccountId;

                _ignoreNextItemSetUpdate = true;

                await App.LeagueClient.ItemSets.UpdateItemSetsAsync(summonerInfo.SummonerId, DataStore.ItemSets);

                ViewModel.ItemSetsMessage = "Item sets synced!";
            }
        }

        private async Task SaveRunePagesAsync()
        {
            if (ViewModel.RuneChampionContext != null)
            {
                var pages = await App.LeagueClient.Perks.GetPagesAsync();
                DataStore.RunePages[ViewModel.RuneChampionContext.Value] = pages.Where(x => x.IsDeletable).ToArray();
                await DataStore.SaveAsync();
            }
        }

        private async Task ItemSetsChangedAsync(long summonerId, ItemSetList itemSets)
        {
            if (_ignoreNextItemSetUpdate)
            {
                _ignoreNextItemSetUpdate = false;
                return;
            }

            DataStore.ItemSets = itemSets;
            await DataStore.SaveAsync();
            ViewModel.ItemSetsMessage = "Item sets saved.";
        }

        private void OpenApiPlayground()
        {
            Interface.Popup<ApiPlaygroundViewModel>();
        }

        private async Task GameflowPhaseChangedAsync(GameflowPhase newPhase)
        {
            if (newPhase == GameflowPhase.ChampSelect)
            {
                // do nothing, this is handled by ChampSelectSessionChanged
            }
            else if (ViewModel.ClientState == GameflowPhase.ChampSelect)
            {
                _attemptedTrades.Clear();
                _skinSetForChampionId = null;
                _currentChampionBaseSkin = default;
                ViewModel.ChampionSelectSession = null;
                ViewModel.IsInAram = false;
            }

            ViewModel.ClientState = newPhase;

            if (newPhase == GameflowPhase.ReadyCheck)
            {
                await App.LeagueClient.Matchmaking.AcceptReadyCheckAsync();
            }
            else if (newPhase is GameflowPhase.PreEndOfGame or GameflowPhase.EndOfGame or GameflowPhase.Lobby or GameflowPhase.Matchmaking && ViewModel.RuneChampionContext != null)
            {
                ViewModel.RuneChampionContext = null;
            }
        }

        private async Task ChampSelectSessionChanged(ChampSelectSession session)
        {
            if (session.IsSpectating)
            {
                return;
            }

            var localPlayer = session.MyTeam.First(x => x.CellId == session.LocalPlayerCellId);

            if (ViewModel.ChampionSelectSession == null)
            {
                ViewModel.RuneChampionContext = null;

                ViewModel.ChampionSelectSession = new ChampionSelectSessionViewModel
                {
                    Position = localPlayer.Position
                };
            }

            var selectedChampion = localPlayer.ChampionId == default ? localPlayer.ChampionPickIntent : localPlayer.ChampionId;

            if (selectedChampion != default && selectedChampion != ViewModel.RuneChampionContext)
            {
                ViewModel.RuneChampionContext = selectedChampion;
            }

            // ARAM or ARURF
            if (session.AllowRerolling && session.BenchEnabled)
            {
                ViewModel.IsInAram = true;

                var currentlyTrading = session.Trades.Any(x => x.State is ChampSelectTradeState.Sent or ChampSelectTradeState.Received);

                if (currentlyTrading)
                {
                    return;
                }

                var currentRank = GetAramChampionRank(localPlayer.ChampionId);
                var highestRankBenchChamp = session.BenchChampionIds.Select(x => new { Rank = GetAramChampionRank(x), ChampionId = x }).OrderBy(x => x.Rank).LastOrDefault();
                var highestRankAllyChamp =
                    session.MyTeam
                        .Select(x => new { Rank = GetAramChampionRank(x.ChampionId), CellId = x.CellId, ChampionId = x.ChampionId })
                        .Where(x => x.CellId != localPlayer.CellId)                                                             // exclude me
                        .Where(x => !_attemptedTrades.Contains((x.CellId, x.ChampionId)))                                       // only attempt to trade once per ally
                        .Where(x => session.Trades.First(y => y.CellId == x.CellId).State == ChampSelectTradeState.Available)   // exclude allies that rejected trade requests
                        .OrderBy(x => x.Rank)
                        .LastOrDefault();

                if (highestRankBenchChamp != null && highestRankBenchChamp.Rank > highestRankAllyChamp.Rank && highestRankBenchChamp.Rank > currentRank)
                {
                    await App.LeagueClient.ChampSelect.SwapWithBenchAsync(highestRankBenchChamp.ChampionId);
                }
                else if (highestRankAllyChamp.Rank > currentRank)
                {
                    // trade id is generated when loading into champ select and is unique for each Player -> Player combination.
                    // all entries in the Trades list are created immediately and states are set to Avaiable.
                    var tradeId = session.Trades.First(x => x.CellId == highestRankAllyChamp.CellId).Id;
                    _attemptedTrades.Add((highestRankAllyChamp.CellId, highestRankAllyChamp.ChampionId));
                    await App.LeagueClient.ChampSelect.RequestTradeAsync(tradeId);
                }
            }

            // Re-select the last selected skin for the champion - if it is available
            IList<SkinSelectorSkin> championSkins;

            if (localPlayer.ChampionId != default && (championSkins = await App.LeagueClient.ChampSelect.GetSkinCarouselSkinsAsync()).Any())
            {
                if (localPlayer.ChampionId != _skinSetForChampionId)
                {
                    _skinSetForChampionId = localPlayer.ChampionId;
                    _currentChampionBaseSkin = championSkins.First(x => x.IsBase).Id;

                    if (DataStore.SelectedSkins.TryGetValue(localPlayer.ChampionId, out var skinId))
                    {
                        foreach (var skin in championSkins)
                        {
                            if (skin.Id == skinId)
                            {
                                if (skin.Unlocked)
                                {
                                    await App.LeagueClient.ChampSelect.PatchMySelectionAsync(new MySelection { SelectedSkinId = skinId });
                                    break;
                                }
                            }

                            foreach (var childSkin in skin.ChildSkins)
                            {
                                if (childSkin.Id == skinId)
                                {
                                    if (childSkin.Unlocked)
                                    {
                                        await App.LeagueClient.ChampSelect.PatchMySelectionAsync(new MySelection { SelectedSkinId = skinId });
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (localPlayer.SelectedSkinId != _currentChampionBaseSkin)
                    {
                        DataStore.SelectedSkins[localPlayer.ChampionId] = localPlayer.SelectedSkinId;
                        await DataStore.SaveAsync();
                    }
                }
                else if (localPlayer.SelectedSkinId != _currentChampionBaseSkin && localPlayer.SelectedSkinId != DataStore.SelectedSkins[localPlayer.ChampionId])
                {
                    DataStore.SelectedSkins[localPlayer.ChampionId] = localPlayer.SelectedSkinId;
                    await DataStore.SaveAsync();
                }
            }
        }

        private void EditAramPreferences()
        {
            var preferredChampions = new ObservableCollection<Champion>(DataStore.PreferredAramChampions.Select(x => ViewModel.Champions.First(y => y.Key == x.ToString())));
            var otherChampions = new ObservableCollection<Champion>(ViewModel.Champions.Except(preferredChampions));

            Interface.Popup(
                new AramPreferenceViewModel
                {
                    OtherChampions = otherChampions,
                    PreferredChampions = preferredChampions
                });
        }

        private async Task ApplyChampionRunesAsync()
        {
            var championRunePages =
                ViewModel.RuneChampionContext != null && DataStore.RunePages.ContainsKey(ViewModel.RuneChampionContext.Value)
                    ? DataStore.RunePages[ViewModel.RuneChampionContext.Value]
                    : Array.Empty<PerkPage>();

            await App.LeagueClient.Perks.DeleteAllPagesAsync();

            if (ViewModel.RuneChampionContext == null || championRunePages.Length == 0)
            {
                return;
            }

            foreach (var page in championRunePages)
            {
                page.Id = (await App.LeagueClient.Perks.CreatePageAsync(page)).Id;
            }

            await App.LeagueClient.Perks.SetCurrentPageAsync(championRunePages[0].Id);
        }

        private int GetAramChampionRank(int championId)
        {
            var index = DataStore.PreferredAramChampions.IndexOf(championId);
            return index == -1 ? 0 : (1000 - index);
        }
    }
}
