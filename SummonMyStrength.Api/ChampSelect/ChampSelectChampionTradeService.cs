using SummonMyStrength.Api.ChampSelect.Trades;
using SummonMyStrength.Api.Connectors;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect;

internal class ChampSelectChampionTradeService : IChampSelectChampionTradeService
{
    private readonly ILeagueClientApiConnector _clientApiConnector;

    public ChampSelectChampionTradeService(ILeagueClientApiConnector clientApiConnector)
    {
        _clientApiConnector = clientApiConnector;
    }

    /// <remarks>
    /// It appears that all trade ids are a number incrementing from 0 unique across all players in champ select. Trade ids
    /// will thus be in the ranges of 0-3, 4-7, etc (since there are 4 players besides yourself).
    /// </remarks>
    public async Task<ChampSelectTradeContract> RequestTradeAsync(long id)
    {
        return await _clientApiConnector.PostAsync<ChampSelectTradeContract>($"lol-champ-select/v1/session/trades/{id}/request", null);
    }

    /// <summary>
    /// Retrieves the details of an ongoing trade request.
    /// </summary>
    /// <returns>The details of the trade.</returns>
    public async Task<OngoingTrade> GetOngoingTradeAsync()
    {
        return await _clientApiConnector.PostAsync<OngoingTrade>($"lol-champ-select/v1/ongoing-trade", null);
    }

    /// <summary>
    /// Accepts the trade request.
    /// </summary>
    /// <param name="id">Either your trade id or your counter part's trade id.</param>
    /// <returns>The task for the action.</returns>
    public async Task AcceptTradeAsync(long id)
    {
        await _clientApiConnector.PostAsync($"lol-champ-select/v1/session/trades/{id}/accept", null);
    }

    /// <summary>
    /// Rejects the trade request.
    /// </summary>
    /// <param name="id">Either your trade id or your counter part's trade id.</param>
    /// <returns>The task for the action.</returns>
    public async Task DeclineTradeAsync(long id)
    {
        await _clientApiConnector.PostAsync($"lol-champ-select/v1/session/trades/{id}/decline", null);
    }
}
