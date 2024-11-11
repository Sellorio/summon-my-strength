using SummonMyStrength.Api.Connectors;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PowerSystems.SummonerSpells;

internal class SummonerSpellSelectionService : ISummonerSpellSelectionService
{
    private readonly ILeagueClientApiConnector _clientApiConnector;

    public SummonerSpellSelectionService(ILeagueClientApiConnector clientApiConnector)
    {
        _clientApiConnector = clientApiConnector;
    }

    public async Task SelectSummonerSpellsAsync(SummonerSpell leftSpell, SummonerSpell rightSpell)
    {
        await _clientApiConnector.PatchAsync("lol-champ-select/v1/session/my-selection", new { Spell1Id = leftSpell.Id, Spell2Id = rightSpell.Id });
    }
}
