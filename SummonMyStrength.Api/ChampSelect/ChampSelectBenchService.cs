using SummonMyStrength.Api.Connectors;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect;

internal class ChampSelectBenchService : IChampSelectBenchService
{
    private readonly ILeagueClientApiConnector _clientApiConnector;

    public ChampSelectBenchService(ILeagueClientApiConnector clientApiConnector)
    {
        _clientApiConnector = clientApiConnector;
    }

    public async Task SwapWithBenchAsync(int championId)
    {
        await _clientApiConnector.PostAsync($"lol-champ-select/v1/session/bench/swap/{championId}", null);
    }
}
