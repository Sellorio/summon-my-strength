using SummonMyStrength.Api.DataDragon;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Champions
{
    public class ChampionsModule
    {
        private readonly LeagueClient _client;
        private string _dataDragonVersion;
        private Champion[] _championCache;

        public ChampionsModule(LeagueClient client)
        {
            _client = client;
        }

        public async Task<Champion[]> GetChampionsAsync()
        {
            if (_championCache != null)
            {
                return _championCache;
            }

            var version = _dataDragonVersion ??= JsonSerializer.Deserialize<string[]>(await _client.DataDragonHttpClient.GetStringAsync("api/versions.json"))[0];
            var json = await _client.DataDragonHttpClient.GetStringAsync($"cdn/{version}/data/en_US/champion.json");
            var championData = JsonSerializer.Deserialize<DataWrapper<Dictionary<string, Champion>>>(json, LeagueClient.JsonSerializerOptions);

            return _championCache = championData.Data.Values.ToArray();
        }

        public string GetIconUrl(Champion champion)
        {
            return $"{_client.DataDragonHttpClient.BaseAddress.AbsoluteUri}/cdn/{_dataDragonVersion}/img/champion/{champion.Image.Full}";
        }
    }
}
