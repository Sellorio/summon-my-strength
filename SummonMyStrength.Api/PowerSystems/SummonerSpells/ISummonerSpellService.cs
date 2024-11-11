using System.Threading.Tasks;

namespace SummonMyStrength.Api.PowerSystems.SummonerSpells;

public interface ISummonerSpellService
{
    string GetIconUrl(SummonerSpell summonerSpell);
    Task<SummonerSpell[]> GetSummonerSpellsAsync();
}
