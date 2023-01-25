using SummonMyStrength.Api;
using SummonMyStrength.Api.Gameflow;
using SummonMyStrength.Api.Items;
using SummonMyStrength.Maui.Services.LiveApi;
using System.Security.Authentication;
using System.Text.Json;
using Timer = System.Timers.Timer;

namespace SummonMyStrength.Maui.Services
{
    internal class GameInfoAccessor : IGameInfoAccessor
    {
        private static readonly string _saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Summon My Strength");
        private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
        private readonly Timer _timer = new(100);
        private readonly LeagueClient _leagueClient;
        private readonly HttpClient _httpClient =
            new(
                new HttpClientHandler
                {
                    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
                    ServerCertificateCustomValidationCallback = (a, b, c, d) => true
                })
            {
                BaseAddress = new Uri("https://127.0.0.1:2999/liveclientdata/")
            };

        private string _gameInfoRaw;

        private int _nextTick = 1;
        private string _championName;
        private Dictionary<string, Item> _items;
        private List<(int? Level, int? Gold)> _data;

        public GameInfo GameInfo { get; private set; }

        public bool SaveStatisticsOnEnd { get; set; }

        public event Func<Task> GameInfoChanged;

        public GameInfoAccessor(LeagueClient leagueClient)
        {
            _leagueClient = leagueClient;

            _timer.Elapsed += Timer_Elapsed;
            _leagueClient.Gameflow.GameflowPhaseChanged += Gameflow_GameflowPhaseChanged;
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
            _items ??= await _leagueClient.Items.GetItemsAsync();

            if (newPhase == GameflowPhase.InProgress)
            {
                await UpdateGameInfoAsync();
                _timer.Enabled = true;
            }
            else
            {
                _timer.Enabled = false;

                if (GameInfo != null)
                {
                    await HandleGameEndedAsync();
                }
            }
        }

        private async Task UpdateGameInfoAsync()
        {
            HttpResponseMessage httpResponse;

            try
            {
                httpResponse = await _httpClient.GetAsync("allgamedata");
            }
            catch
            {
                if (GameInfo != null)
                {
                    await HandleGameEndedAsync();
                }

                return;
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                if (GameInfo != null)
                {
                    await HandleGameEndedAsync();
                }

                return;
            }

            string newGameInfoJson = await httpResponse.Content.ReadAsStringAsync();

            if (newGameInfoJson != _gameInfoRaw)
            {
                _gameInfoRaw = newGameInfoJson;
                GameInfo = JsonSerializer.Deserialize<GameInfo>(_gameInfoRaw, _jsonSerializerOptions);
                UpdateStatistics();
                await GameInfoChanged.InvokeAsync();
            }
        }

        private void UpdateStatistics()
        {
            if (GameInfo != null && GameInfo.GameSettings.GameMode == "CLASSIC")
            {
                var gameTimeAsTicks = (int)Math.Floor(GameInfo.GameSettings.GameTime / 30);

                if (_nextTick <= gameTimeAsTicks)
                {
                    var currentPlayer = GameInfo.AllPlayers.First(x => x.SummonerName == GameInfo.ActivePlayer.SummonerName);

                    _data ??= new();
                    _championName ??= currentPlayer.ChampionName;

                    var gold = (int)GameInfo.ActivePlayer.CurrentGold + currentPlayer.Items.Sum(x => _items[x.ItemId.ToString()].Gold.Total);

                    _data.Add(new(GameInfo.ActivePlayer.Level, gold));

                    _nextTick = gameTimeAsTicks + 1;
                }
            }
        }

        private async Task HandleGameEndedAsync()
        {
            if (SaveStatisticsOnEnd && _data != null)
            {
                SaveStatisticsOnEnd = false;

                await SaveStatisticsAsync("Gold", _data.Select(x => x.Gold));
                await SaveStatisticsAsync("Level", _data.Select(x => x.Level));
            }

            _nextTick = 1;
            _championName = null;
            _data = null;

            _gameInfoRaw = null;
            GameInfo = null;

            await GameInfoChanged.InvokeAsync();
        }

        private async Task SaveStatisticsAsync<T>(string statisticName, IEnumerable<T> values)
        {
            string filename = Path.Combine(_saveDirectory, $"{_championName}_{statisticName}.csv");
            string content = "";

            if (!File.Exists(filename))
            {
                int gameTime = 0;
                content = string.Join(",", Enumerable.Repeat(0, 90).Select(x => TimeSpan.FromSeconds(gameTime += 30).ToString("mm\\:ss"))) + "\r\n";
            }

            values = values.Concat(Enumerable.Repeat<T>(default, 90)).Take(90);
            content += string.Join(",", values) + "\r\n";

            await File.AppendAllTextAsync(filename, content);
        }
    }
}
