using SummonMyStrength.Api.DataDragon;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.SummonerSpells
{
    public class SummonerSpellsModule
    {
        private readonly LeagueClient _client;
        private string _dataDragonVersion;
        private SummonerSpell[] _spellCache;

        public SummonerSpellsModule(LeagueClient client)
        {
            _client = client;
        }

        public async Task<SummonerSpell[]> GetSummonerSpellsAsync()
        {
            if (_spellCache != null)
            {
                return _spellCache;
            }

            var version = _dataDragonVersion ??= JsonSerializer.Deserialize<string[]>(await _client.DataDragonHttpClient.GetStringAsync("api/versions.json"))[0];
            var json = await _client.DataDragonHttpClient.GetStringAsync($"cdn/{version}/data/en_US/summoner.json");
            var championData = JsonSerializer.Deserialize<DataWrapper<Dictionary<string, SummonerSpell>>>(json, LeagueClient.JsonSerializerOptions);

            return _spellCache = championData.Data.Values.ToArray();
        }

        public string GetIconUrl(SummonerSpell spell)
        {
            return $"{_client.DataDragonHttpClient.BaseAddress.AbsoluteUri}cdn/{_dataDragonVersion}/img/spell/{spell.Code}.png";
        }
    }
}
