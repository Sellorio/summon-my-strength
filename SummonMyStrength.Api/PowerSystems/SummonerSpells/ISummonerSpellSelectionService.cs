using System.Threading.Tasks;

namespace SummonMyStrength.Api.PowerSystems.SummonerSpells;

public interface ISummonerSpellSelectionService
{
    Task SelectSummonerSpellsAsync(SummonerSpell leftSpell, SummonerSpell rightSpell);
}
