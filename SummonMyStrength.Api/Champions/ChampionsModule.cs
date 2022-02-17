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

        public ChampionsModule(LeagueClient client)
        {
            _client = client;
        }

        public async Task<Champion[]> GetChampionsAsync()
        {
            var version = JsonSerializer.Deserialize<string[]>(await _client.DataDragonHttpClient.GetStringAsync("api/versions.json"))[0];
            var json = await _client.DataDragonHttpClient.GetStringAsync($"cdn/{version}/data/en_US/champion.json");
            var championData = JsonSerializer.Deserialize<DataWrapper<Dictionary<string, Champion>>>(json, _client.JsonSerializerOptions);

            return championData.Data.Values.ToArray();
        }
    }
}
