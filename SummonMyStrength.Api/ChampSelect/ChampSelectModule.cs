using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect
{
    public sealed class ChampSelectModule
    {
        private readonly LeagueClient _client;

        public event Action<ChampSelectSession> SessionChanged;

        internal ChampSelectModule(LeagueClient client)
        {
            _client = client;

            _client.AddMessageHandler(x =>
            {
                if (x.Path == "/lol-lobby-team-builder/champ-select/v1/session" && SessionChanged != null)
                {
                    SessionChanged.Invoke(JsonSerializer.Deserialize<ChampSelectSession>(x.Data.GetRawText(), _client.JsonSerializerOptions));
                }
            });
        }

        public async Task<ChampSelectSession> GetSessionAsync()
        {
            return
                JsonSerializer.Deserialize<ChampSelectSession>(
                    await _client.HttpClient.GetStringAsync("lol-champ-select/v1/session"),
                    _client.JsonSerializerOptions);
        }
    }
}
