using SummonMyStrength.Api;
using SummonMyStrength.Api.Gameflow;
using SummonMyStrength.Maui.Services.LiveApi;
using System.Security.Authentication;
using System.Text.Json;
using Timer = System.Timers.Timer;

namespace SummonMyStrength.Maui.Services
{
    internal class GameInfoAccessor : IGameInfoAccessor
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
        private readonly Timer _timer = new(30.0);
        private readonly HttpClient _httpClient =
            new(
                new HttpClientHandler
                {
                    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
                    ServerCertificateCustomValidationCallback = (a, b, c, d) => true
                })
            {
                BaseAddress = new Uri("https://127.0.0.1:2999/liveclientdata")
            };

        private string _gameInfoRaw;

        public GameInfo GameInfo { get; private set; }

        public event Func<Task> GameInfoChanged;

        public GameInfoAccessor(LeagueClient leagueClient)
        {
            _timer.Elapsed += Timer_Elapsed;
            leagueClient.Gameflow.GameflowPhaseChanged += Gameflow_GameflowPhaseChanged;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Enabled = false;

            Task.Factory.StartNew(() =>
            {
                UpdateGameInfoAsync().GetAwaiter().GetResult();
                _timer.Enabled = true;
            });
        }

        private async Task Gameflow_GameflowPhaseChanged(GameflowPhase newPhase)
        {
            if (newPhase == GameflowPhase.InProgress)
            {
                await UpdateGameInfoAsync();
                _timer.Enabled = true;
            }
            else
            {
                _timer.Enabled = false;
                _gameInfoRaw = null;
                GameInfo = null;
            }
        }

        private async Task UpdateGameInfoAsync()
        {
            var httpResponse = await _httpClient.GetAsync("allgamedata");

            if (!httpResponse.IsSuccessStatusCode)
            {
                if (GameInfo != null)
                {
                    _gameInfoRaw = null;
                    GameInfo = null;
                }

                return;
            }

            string newGameInfoJson = await httpResponse.Content.ReadAsStringAsync();

            if (newGameInfoJson != _gameInfoRaw)
            {
                _gameInfoRaw = newGameInfoJson;
                GameInfo = JsonSerializer.Deserialize<GameInfo>(_gameInfoRaw, _jsonSerializerOptions);
                await GameInfoChanged.InvokeAsync();
            }
        }
    }
}
