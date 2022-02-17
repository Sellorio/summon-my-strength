using SEA.Mvvm.ModelSupport;
using SummonMyStrength.Api;
using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Api.Gameflow;
using SummonMyStrength.Api.Perks;
using SummonMyStrength.Data;
using SummonMyStrength.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummonMyStrength.Models
{
    class MainModel : ModelBase<MainViewModel>
    {
        public MainModel(IInterface @interface, MainViewModel viewModel)
            : base(@interface, viewModel)
        {
        }

        protected override void BindModel(ModelBinder<MainViewModel> modelBinder)
        {
            modelBinder.BindAsync(x => x.Initialise, InitialiseAsync);
            modelBinder.Bind(x => x.OpenApiPlayground, OpenApiPlayground);
            modelBinder.BindAsync(x => x.ReApplyChampionRunes, ApplyChampionRunesAsync);

            modelBinder.BindChange(x => x.RuneChampionContext, () => ViewModel.ReApplyChampionRunes.Execute(null));
        }
        
        private async Task InitialiseAsync()
        {
            App.LeagueClient = new LeagueClient();
            await App.LeagueClient.ConnectAsync();

            ViewModel.Champions = await App.LeagueClient.Champions.GetChampionsAsync();

            if (App.LeagueClient.IsConnected)
            {
                await GameflowPhaseChangedAsync(await App.LeagueClient.Gameflow.GetGameflowPhaseAsync());
            }

            App.LeagueClient.Gameflow.GameflowPhaseChanged += x => _ = GameflowPhaseChangedAsync(x);
            App.LeagueClient.Perks.PerkPagesUpdated += x => _ = AllRunePagesUpdatedAsync(x);
            App.LeagueClient.Perks.PerkPageUpdated += x => _ = RunePageUpdatedAsync(x);
            App.LeagueClient.ChampSelect.SessionChanged += ChampSelectSessionChanged;
        }

        private async Task RunePageUpdatedAsync(PerkPage runePage)
        {
            if (ViewModel.ClientState != GameflowPhase.ChampSelect && ViewModel.RuneChampionContext != null && DataStore.RunePages.ContainsKey(ViewModel.RuneChampionContext.Value))
            {
                var savedRunePages = DataStore.RunePages[ViewModel.RuneChampionContext.Value];
                var savedRunePage = savedRunePages.FirstOrDefault(x => x.Id == runePage.Id);

                if (savedRunePage != null)
                {
                    savedRunePages[savedRunePages.IndexOf(savedRunePage)] = runePage;
                    await DataStore.SaveAsync();
                }
            }
        }

        private async Task AllRunePagesUpdatedAsync(PerkPage[] runePages)
        {
            if (ViewModel.ClientState != GameflowPhase.ChampSelect && ViewModel.RuneChampionContext != null)
            {
                DataStore.RunePages[ViewModel.RuneChampionContext.Value] = runePages.Where(x => x.IsEditable).ToList();
                await DataStore.SaveAsync();
            }
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
                ViewModel.ChampionSelectSession = null;
            }

            if (newPhase == GameflowPhase.ReadyCheck)
            {
                await App.LeagueClient.Matchmaking.AcceptReadyCheckAsync();
            }

            ViewModel.ClientState = newPhase;
        }

        private void ChampSelectSessionChanged(ChampSelectSession session)
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
        }

        private async Task ApplyChampionRunesAsync()
        {
            await App.LeagueClient.Perks.DeleteAllPagesAsync();

            if (ViewModel.RuneChampionContext == null)
            {
                return;
            }

            var championRunePages =
                DataStore.RunePages.ContainsKey(ViewModel.RuneChampionContext.Value)
                    ? DataStore.RunePages[ViewModel.RuneChampionContext.Value]
                    : new List<PerkPage>();

            foreach (var page in championRunePages)
            {
                page.Id = (await App.LeagueClient.Perks.CreatePageAsync(page)).Id;
            }

            var firstPageId = championRunePages.OrderBy(x => x.Name).FirstOrDefault()?.Id;

            if (firstPageId != null)
            {
                await App.LeagueClient.Perks.SetCurrentPageAsync(firstPageId.Value);
            }
        }
    }
}
