using SummonMyStrength.Api.Connectors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PowerSystems.SummonerSpells;

internal class SummonerSpellService : ISummonerSpellService
{
    private readonly IDataDragonApiConnector _dataDragonApiConnector;

    public SummonerSpellService(IDataDragonApiConnector dataDragonApiConnector)
    {
        _dataDragonApiConnector = dataDragonApiConnector;
    }

    public async Task<SummonerSpell[]> GetSummonerSpellsAsync()
    {
        var result = await _dataDragonApiConnector.GetDataAsync<Dictionary<string, SummonerSpell>>("summoner.json");
        return result.Values.ToArray();
    }

    public string GetIconUrl(SummonerSpell summonerSpell)
    {
        return _dataDragonApiConnector.GetIconUrl($"spell/{summonerSpell.Code}.png");
    }
}
