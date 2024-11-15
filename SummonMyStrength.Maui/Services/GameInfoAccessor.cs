﻿//using SummonMyStrength.Api;
//using SummonMyStrength.Api.General;
//using SummonMyStrength.Maui.Services.LiveApi;
//using System.Security.Authentication;
//using System.Text.Json;
//using Timer = System.Timers.Timer;

//namespace SummonMyStrength.Maui.Services;

//internal class GameInfoAccessor : IGameInfoAccessor
//{
//    private static readonly string _saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Summon My Strength");
//    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
//    private readonly Timer _timer = new(100);
//    private readonly LeagueClient _leagueClient;
//    private readonly HttpClient _httpClient =
//        new(
//            new HttpClientHandler
//            {
//                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
//                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
//            })
//        {
//            BaseAddress = new Uri("https://127.0.0.1:2999/liveclientdata/")
//        };

//    private string _gameInfoRaw;

//    private int _nextTick = 1;
//    private string _championName;
//    private List<(int? Level, int? Gold)> _data;
//    private bool _isUpdating;

//    public GameInfo GameInfo { get; private set; }

//    public bool SaveStatisticsOnEnd { get; set; }

//    public event Func<Task> GameInfoChanged;

//    public GameInfoAccessor(LeagueClient leagueClient)
//    {
//        _leagueClient = leagueClient;

//        _timer.Elapsed += Timer_Elapsed;
//        _leagueClient.Gameflow.GameflowPhaseChanged += Gameflow_GameflowPhaseChanged;
//    }

//    private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
//    {
//        if (!_isUpdating)
//        {
//            _isUpdating = true;

//            Task.Factory.StartNew(() =>
//            {
//                UpdateGameInfoAsync().GetAwaiter().GetResult();
//                _isUpdating = false;
//            });
//        }
//    }

//    private async Task Gameflow_GameflowPhaseChanged(GameflowPhase newPhase)
//    {
//        if (newPhase == GameflowPhase.InProgress)
//        {
//            await Task.Delay(2000);
//            await UpdateGameInfoAsync();
//            _timer.Enabled = true;
//        }
//        else
//        {
//            _timer.Enabled = false;

//            if (GameInfo != null)
//            {
//                await HandleGameEndedAsync();
//            }
//        }
//    }

//    private async Task<bool> UpdateGameInfoAsync()
//    {
//        HttpResponseMessage httpResponse;

//        try
//        {
//            httpResponse = await _httpClient.GetAsync("allgamedata");
//        }
//        catch
//        {
//            if (GameInfo != null)
//            {
//                await HandleGameEndedAsync();
//            }

//            return false;
//        }

//        if (!httpResponse.IsSuccessStatusCode)
//        {
//            if (GameInfo != null)
//            {
//                await HandleGameEndedAsync();
//            }

//            return false;
//        }

//        string newGameInfoJson = await httpResponse.Content.ReadAsStringAsync();

//        if (newGameInfoJson != _gameInfoRaw)
//        {
//            _gameInfoRaw = newGameInfoJson;

//            try
//            {
//                // throws exception when exiting practice tool
//                GameInfo = JsonSerializer.Deserialize<GameInfo>(_gameInfoRaw, _jsonSerializerOptions);
//            }
//            catch
//            {
//                return false;
//            }

//            await GameInfoChanged.InvokeAsync();
//        }

//        return true;
//    }

//    private async Task HandleGameEndedAsync()
//    {
//        if (SaveStatisticsOnEnd && _data != null)
//        {
//            SaveStatisticsOnEnd = false;

//            await SaveStatisticsAsync("Gold", _data.Select(x => x.Gold));
//            await SaveStatisticsAsync("Level", _data.Select(x => x.Level));
//        }

//        _nextTick = 1;
//        _championName = null;
//        _data = null;

//        _gameInfoRaw = null;
//        GameInfo = null;

//        await GameInfoChanged.InvokeAsync();
//    }

//    private async Task SaveStatisticsAsync<T>(string statisticName, IEnumerable<T> values)
//    {
//        string filename = Path.Combine(_saveDirectory, $"{_championName}_{statisticName}.csv");
//        string content = "";

//        if (!File.Exists(filename))
//        {
//            int gameTime = 0;
//            content = string.Join(",", Enumerable.Repeat(0, 90).Select(x => TimeSpan.FromSeconds(gameTime += 30).ToString("mm\\:ss"))) + "\r\n";
//        }

//        values = values.Concat(Enumerable.Repeat<T>(default, 90)).Take(90);
//        content += string.Join(",", values) + "\r\n";

//        await File.AppendAllTextAsync(filename, content);
//    }
//}
