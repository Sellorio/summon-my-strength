using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Login
{
    public class LoginModule
    {
        private readonly LeagueClient _client;

        internal LoginModule(LeagueClient client)
        {
            _client = client;
        }

        public async Task<LoginSession> GetSession()
        {
            return JsonSerializer.Deserialize<LoginSession>(await _client.HttpClient.GetStringAsync("lol-login/v1/session"), LeagueClient.JsonSerializerOptions);
        }
    }
}
