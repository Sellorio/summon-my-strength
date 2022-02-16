using System.Net.Http;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Matchmaking
{
    public class MatchmakingModule
    {
        private readonly LeagueClient _client;

        internal MatchmakingModule(LeagueClient client)
        {
            _client = client;
        }

        public async Task AcceptReadyCheckAsync()
        {
            var response = await _client.HttpClient.PostAsync("lol-matchmaking/v1/ready-check/accept", new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();
        }
    }
}
