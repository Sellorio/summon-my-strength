using System.Collections.Generic;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Data
{
    public static class SummonerSpellData
    {
        public static IReadOnlyList<SummonerSpell> SummonerSpells { get; private set; }

        public static async Task LoadAsync()
        {

        }
    }
}
