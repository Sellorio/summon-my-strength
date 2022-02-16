using SEA.Mvvm.ModelSupport;
using SummonMyStrength.Api;
using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Api.Gameflow;
using SummonMyStrength.ViewModels;
using System.Linq;

namespace SummonMyStrength.Models
{
    class MainModel : ModelBase<MainViewModel>
    {
        private readonly LeagueClient _leagueClient = new();

        public MainModel(IInterface @interface, MainViewModel viewModel)
            : base(@interface, viewModel)
        {
        }

        protected override void BindModel(ModelBinder<MainViewModel> modelBinder)
        {
            _ = _leagueClient.ConnectAsync();
            _leagueClient.Gameflow.GameflowPhaseChanged += GameflowPhaseChanged;
        }

        private async void GameflowPhaseChanged(GameflowPhase newPhase)
        {
            if (newPhase == GameflowPhase.ChampSelect)
            {
                var session = await _leagueClient.ChampSelect.GetSessionAsync();

                if (session.IsSpectating)
                {
                    return;
                }

                ViewModel.ChampionSelectSession = new ChampionSelectSessionViewModel();
                ViewModel.ChampionSelectSession.Position = session.MyTeam.First(x => x.CellId == session.LocalPlayerCellId).Position;
            }
            else if (ViewModel.ClientState == GameflowPhase.ChampSelect)
            {
                ViewModel.ChampionSelectSession = null;
            }

            if (newPhase == GameflowPhase.ReadyCheck)
            {
                await _leagueClient.Matchmaking.AcceptReadyCheckAsync();
            }

            ViewModel.ClientState = newPhase;
        }

        //private void CheckForRoleInChampSelectSession(ChampSelectSession session)
        //{
        //    var position = session.MyTeam.First(x => x.CellId == session.LocalPlayerCellId).Position;

        //    if (position != null)
        //    {
        //        ViewModel.ChampionSelectSession.Position = position;
        //        _leagueClient.ChampSelect.SessionChanged -= CheckForRoleInChampSelectSession;
        //    }
        //}
    }
}
