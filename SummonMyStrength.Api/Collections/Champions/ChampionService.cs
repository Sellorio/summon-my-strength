using SummonMyStrength.Api.Connectors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Collections.Champions;

internal class ChampionService : IChampionService
{
    private readonly IDataDragonApiConnector _dataDragonApiConnector;

    public ChampionService(IDataDragonApiConnector dataDragonApiConnector)
    {
        _dataDragonApiConnector = dataDragonApiConnector;
    }

    public async Task<Champion[]> GetChampionsAsync()
    {
        var result = await _dataDragonApiConnector.GetDataAsync<Dictionary<string, Champion>>("champion.json");
        return result.Values.ToArray();
    }

    public string GetIconUrl(Champion champion)
    {
        return _dataDragonApiConnector.GetIconUrl($"champion/{champion.Image.Full}");
    }
}
