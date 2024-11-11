using SummonMyStrength.Api.Connectors;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect;

internal class ChampSelectPickOrderSwapService : IChampSelectPickOrderSwapService
{
    private readonly ILeagueClientApiConnector _clientApiConnector;

    public ChampSelectPickOrderSwapService(ILeagueClientApiConnector clientApiConnector)
    {
        _clientApiConnector = clientApiConnector;
    }

    /// <summary>
    /// Accepts the swap request. Trade is when trading champions. Swap is when trading pick order.
    /// </summary>
    /// <param name="id">Either your trade id or your counter part's swap id.</param>
    /// <returns>The task for the action.</returns>
    public async Task AcceptSwapAsync(long id)
    {
        await _clientApiConnector.PostAsync($"lol-champ-select/v1/session/swaps/{id}/accept", null);
    }

    /// <summary>
    /// Rejects the trade request. Trade is when trading champions. Swap is when trading pick order.
    /// </summary>
    /// <param name="id">Either your trade id or your counter part's swap id.</param>
    /// <returns>The task for the action.</returns>
    public async Task DeclineSwapAsync(long id)
    {
        await _clientApiConnector.PostAsync($"lol-champ-select/v1/session/swaps/{id}/decline", null);
    }
}
