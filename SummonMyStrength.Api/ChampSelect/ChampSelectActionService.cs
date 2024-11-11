using SummonMyStrength.Api.Connectors;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect;

internal class ChampSelectActionService : IChampSelectActionService
{
    private readonly ILeagueClientApiConnector _clientApiConnector;

    public ChampSelectActionService(ILeagueClientApiConnector clientApiConnector)
    {
        _clientApiConnector = clientApiConnector;
    }

    public async Task PatchActionAsync(long id, ChampSelectAction action)
    {
        await _clientApiConnector.PatchAsync($"lol-champ-select/v1/session/actions/{id}", action);
    }
}
