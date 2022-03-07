using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Summoner
{
    public class SummonerModule
    {
        private readonly LeagueClient _client;

        internal SummonerModule(LeagueClient client)
        {
            _client = client;
        }

        public async Task<SummonerInfo> GetCurrentSummonerAsync()
        {
            return JsonSerializer.Deserialize<SummonerInfo>(await _client.HttpClient.GetStringAsync("lol-summoner/v1/current-summoner"), _client.JsonSerializerOptions);
        }
    }
}
