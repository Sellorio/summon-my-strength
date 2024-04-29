using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.ChampSelect;
using System.Diagnostics;

namespace SummonMyStrength.Maui.Components.ChampSelect
{
    public partial class ChampionSelectPickBar
    {
        private List<Champion> _champions;
        private bool? _isBanning;
        private Champion _selectedPick;
        private Champion _selectedBan;

        protected override async Task OnInitializedAsync()
        {
            ChampSelectSessionAccessor.SessionChanged += ChampSelectSessionChanged;
            PickBanService.AssignedPositionChanged += AssignedPositionChanged;

            if (ChampSelectSessionAccessor.Session != null)
            {
                await ChampSelectSessionChanged(null, ChampSelectSessionAccessor.Session);
            }
        }
        
        public void Dispose()
        {
            ChampSelectSessionAccessor.SessionChanged -= ChampSelectSessionChanged;
            PickBanService.AssignedPositionChanged -= AssignedPositionChanged;
        }

        private async Task ChampSelectSessionChanged(ChampSelectSession from, ChampSelectSession to)
        {
            if (to != null)
            {
                if (_isBanning != to.IsBanning)
                {
                    await UpdateChampionsListAsync();
                }

                var selectedChampionId = to.Player.ChampionId == default ? to.Player.ChampionPickIntent : to.Player.ChampionId;

                if (selectedChampionId != default && (_selectedPick == null || _selectedPick.Id != selectedChampionId))
                {
                    _selectedPick = _champions.FirstOrDefault(x => x.Id == selectedChampionId);
                }
            }

            await InvokeAsync(StateHasChanged);
        }

        private async Task SelectChampionAsync(Champion champion)
        {
            try
            {
                if (ChampSelectSessionAccessor.Session.IsBanning)
                {
                    _selectedBan = champion;
                    await PickBanService.BanChampionAsync(champion);
                }
                else
                {
                    _selectedPick = champion;
                    await PickBanService.PickChampionAsync(champion);
                }
            }
            catch
            {
            }
        }

        private async Task AssignedPositionChanged()
        {
            await UpdateChampionsListAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateChampionsListAsync()
        {
            var session = ChampSelectSessionAccessor.Session;

            if (session == null)
            {
                return;
            }

            var pick = session.Player.ChampionId == default ? session.Player.ChampionPickIntent : session.Player.ChampionId;

            if (session.IsBanning)
            {
                _isBanning = true;
                _champions = await PickBanService.GetRecentBansAsync();
            }
            else
            {
                _isBanning = false;
                _champions = await PickBanService.GetRecentPicksAsync();
                _selectedPick = _champions.FirstOrDefault(x => x.Id == pick);
            }
        }

        private bool IsChampionDisbled(int championId)
        {
            var session = ChampSelectSessionAccessor.Session;

            if (session == null)
            {
                return true;
            }

            if (session.IsBanning)
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
}
