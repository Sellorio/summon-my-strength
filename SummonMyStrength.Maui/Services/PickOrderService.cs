using SummonMyStrength.Api;
using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Maui.Services.ChampSelect;
using System.Diagnostics;
using System.Text.Json;

namespace SummonMyStrength.Maui.Services
{
    internal class PickOrderService : IPickOrderService
    {
        private readonly LeagueClient _leagueClient;
        private readonly IChampSelectSessionAccessor _champSelectSessionAccessor;

        public PickOrderService(LeagueClient leagueClient, IChampSelectSessionAccessor champSelectSessionAccessor)
        {
            _leagueClient = leagueClient;
            _champSelectSessionAccessor = champSelectSessionAccessor;
            _champSelectSessionAccessor.SessionChanged += OnSessionChanged;
        }

        private async Task OnSessionChanged(ChampSelectSession from, ChampSelectSession to)
        {
            if (to == null)
            {
                return;
            }

            var receivedSwapRequest = to.PickOrderSwaps.FirstOrDefault(x => x.State == ChampSelectTradeState.Received);

            if (receivedSwapRequest == null)
            {
                return;
            }

            switch (DataStore.PickOrderTradeResponse)
            {
                case TradeResponse.None:
                    break;
                case TradeResponse.Accept:
                    await _leagueClient.ChampSelect.AcceptSwapAsync(receivedSwapRequest.Id);
                    break;
                case TradeResponse.Reject:
                    await _leagueClient.ChampSelect.DeclineSwapAsync(receivedSwapRequest.Id);
                    break;
                case TradeResponse.DownOnly:
                    // the order in which players appear in the MyTeam list is also the pick order
                    var isAfterMeInPickOrder = to.MyTeam.Where(x => x.CellId == receivedSwapRequest.CellId || x.CellId == to.LocalPlayerCellId).First().CellId == to.LocalPlayerCellId;

                    if (isAfterMeInPickOrder)
                    {
                        await _leagueClient.ChampSelect.AcceptSwapAsync(receivedSwapRequest.Id);
                    }
                    else
                    {
                        await _leagueClient.ChampSelect.DeclineSwapAsync(receivedSwapRequest.Id);
                    }
                    break;
                default:
                    throw new NotSupportedException("Unexpected trade response type.");
            }
        }
    }
}
