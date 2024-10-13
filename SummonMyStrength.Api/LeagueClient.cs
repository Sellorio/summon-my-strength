using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Management;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using SummonMyStrength.Api.Gameflow;
using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Api.Matchmaking;
using System.Net.WebSockets;
using System.Threading;
using System.Net;
using SummonMyStrength.Api.Perks;
using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.Summoner;
using SummonMyStrength.Api.Login;
using SummonMyStrength.Api.ItemSets;
using System.IO;
using SummonMyStrength.Api.Items;
using SummonMyStrength.Api.SummonerSpells;
using SummonMyStrength.Api.Honors;
using SummonMyStrength.Api.PostGame;
using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.General;

namespace SummonMyStrength.Api;

public sealed class LeagueClient : IDisposable
{
    private static readonly Regex _authTokenRegex = new("\"--remoting-auth-token=(.+?)\"");
    private static readonly Regex _portRegex = new("\"--app-port=(\\d+?)\"");
    private static readonly JsonSerializerOptions _jsonSerializerOptions;

    private static readonly HttpMessageHandler _httpMessageHandler;

    internal static JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions;

    private readonly List<Func<LeagueClientWebSocketMessage, Task>> _webSocketMessageHandlers = new();
    private ClientWebSocket _socketConnection;
    private CancellationTokenSource _cancellationTokenSource;

    public HttpClient HttpClient { get; private set; }

    internal HttpClient DataDragonHttpClient { get; set; }

    public event Func<Task> Connected;
    public event Func<Task> Disconnected;

    public bool IsConnected => _socketConnection?.State == WebSocketState.Open;

    public ChampionsModule Champions { get; }
    public ItemsModule Items { get; }
    public ChampSelectModule ChampSelect { get; }
    public GameflowModule Gameflow { get; }
    public ItemSetModule ItemSets { get; set; }
    public LoginModule Login { get; }
    public MatchmakingModule Matchmaking { get; }
    public PerksModule Perks { get; }
    public SummonerModule Summoner { get; }
    public SummonerSpellsModule SummonerSpells { get; }
    public HonorModule Honors { get; }
    public PostGameModule PostGame { get; }

    static LeagueClient()
    {
        _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        _httpMessageHandler = new HttpClientHandler
        {
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
            ServerCertificateCustomValidationCallback = (a, b, c, d) => true
        };
    }

    public LeagueClient()
    {
        DataDragonHttpClient = new(_httpMessageHandler)
        {
            BaseAddress = new("http://ddragon.leagueoflegends.com/")
        };

        Champions = new(this);
        Items = new(this);
        ChampSelect = new(this);
        Gameflow = new(this);
        ItemSets = new(this);
        Login = new(this);
        Matchmaking = new(this);
        Perks = new(this);
        Summoner = new(this);
        SummonerSpells = new(this);
        Honors = new(this);
        PostGame = new(this);
    }

    public async Task<bool> ConnectAsync()
    {
        // only exists once successfully connected
        await InternalConnectAsync(CancellationToken.None);

        var phase = await Gameflow.GetGameflowPhaseAsync();
        await Gameflow.GameflowPhaseChangedDelegate.InvokeAsync(phase);

        if (phase == GameflowPhase.ChampSelect)
        {
            await ChampSelect.SessionChangedDelegate.InvokeAsync(await ChampSelect.GetSessionAsync());
        }

        return true;
    }

    public void Dispose()
    {
        ((IDisposable)_socketConnection)?.Dispose();
    }

    private async Task InternalConnectAsync(CancellationToken cancellationToken)
    {
        await TryConnectAsync();

        if (IsConnected)
        {
            return;
        }

        do
        {
            await Task.Delay(2000, cancellationToken);
            await TryConnectAsync();
        }
        while (!IsConnected);
    }

    internal void AddMessageHandler(Func<LeagueClientWebSocketMessage, Task> handler)
    {
        if (!_webSocketMessageHandlers.Contains(handler))
        {
            _webSocketMessageHandlers.Add(handler);
        }
    }

    private async Task TryConnectAsync()
    {
        if (IsConnected)
        {
            return;
        }

        var status = GetConnectionSettingsFromLeagueClientProcess();

        if (status == null)
        {
            return;
        }

        try
        {
            var authenticationToken = Convert.ToBase64String(Encoding.ASCII.GetBytes("riot:" + status.Password));

            HttpClient = new(_httpMessageHandler);
            HttpClient.DefaultRequestHeaders.Authorization = new("Basic", authenticationToken);
            HttpClient.BaseAddress = new("https://127.0.0.1:" + status.PortNumber);

            var socketConnection = _socketConnection = new ClientWebSocket();
            _socketConnection.Options.Credentials = new NetworkCredential("riot", status.Password);
            _socketConnection.Options.RemoteCertificateValidationCallback = (a, b, c, d) => true;
            _socketConnection.Options.AddSubProtocol("wamp");

            // cancel this token if we add a Disconnect method to this class
            _cancellationTokenSource = new CancellationTokenSource();

            await _socketConnection.ConnectAsync(new Uri("wss://127.0.0.1:" + status.PortNumber + "/"), CancellationToken.None);

            _ = Task.Factory.StartNew(
                () => ReceiveWebSocketMessagesAsync(socketConnection, _cancellationTokenSource.Token),
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            await _socketConnection.SendAsync(Encoding.UTF8.GetBytes("[5, \"OnJsonApiEvent\"]"), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);

            await Connected.InvokeAsync();
        }
        catch (WebSocketException ex) when (ex.Message.Contains("Unable to connect"))
        {
            HttpClient = null;
            _socketConnection = null;
            _cancellationTokenSource = null;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Exception occurred trying to connect to League of Legends: {e}");
        }
    }

    private async Task ReceiveWebSocketMessagesAsync(ClientWebSocket webSocket, CancellationToken cancellationToken)
    {
        var receiveBytes = new byte[1024];
        var receiveBuffer = new ArraySegment<byte>(receiveBytes);
        StringBuilder currentMessage = new(1000);

        while (true)
        {
            WebSocketReceiveResult receivedMessage;

            try
            {
                receivedMessage = await webSocket.ReceiveAsync(receiveBuffer, cancellationToken);
            }
            catch (WebSocketException ex) when (ex.Message == "The remote party closed the WebSocket connection without completing the close handshake.")
            {
                await HandleDisconnectAsync(cancellationToken);
                return;
            }

            if (receivedMessage.MessageType == WebSocketMessageType.Close)
            {
                await HandleDisconnectAsync(cancellationToken);
                return;
            }
            else if (receivedMessage.MessageType == WebSocketMessageType.Text)
            {
                currentMessage.Append(Encoding.UTF8.GetString(receiveBuffer.Array, 0, receivedMessage.Count));

                if (receivedMessage.EndOfMessage)
                {
                    try
                    {
                        await HandleMessageAsync(currentMessage.ToString());
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Error when handling message:\r\n" + ex);
                    }

                    currentMessage = new(1000);
                }
            }
        }
    }

    private async Task HandleDisconnectAsync(CancellationToken cancellationToken)
    {
        _socketConnection = null;
        HttpClient = null;

        await Disconnected?.InvokeAsync();

        await Task.Delay(2000, cancellationToken);
        await InternalConnectAsync(cancellationToken);
    }

    private async Task HandleMessageAsync(string content)
    {
        var payload = JsonSerializer.Deserialize<JsonElement>(content, _jsonSerializerOptions);

        if (payload.ValueKind != JsonValueKind.Array
            || payload.GetArrayLength() != 3
            || payload[0].ValueKind != JsonValueKind.Number
            || payload[0].GetInt64() != 8
            || payload[1].ValueKind != JsonValueKind.String
            || payload[1].GetString() != "OnJsonApiEvent")
        {
            return;
        }

        var eventData = payload[2];
        var uri = eventData.GetProperty("uri").GetString();

        var webSocketMessage =
            new LeagueClientWebSocketMessage(
                uri,
                eventData.GetProperty("eventType").GetString(),
                eventData.GetProperty("data"));

        if (uri != "/lol-player-report-sender/v1/in-game-reports")
        {
            File.AppendAllText(@"C:\Users\seami\Desktop\league-client-msg-log.txt", uri + "\r\n");
        }

        foreach (var handler in _webSocketMessageHandlers)
        {
            await handler.Invoke(webSocketMessage);
        }
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    private static LeagueConnectionSettings GetConnectionSettingsFromLeagueClientProcess()
    {
        foreach (var process in Process.GetProcessesByName("LeagueClientUx"))
        {
            using var managementObjectSearcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id);
            using var moc = managementObjectSearcher.Get();

            var commandLine = (string)moc.OfType<ManagementObject>().First()["CommandLine"];

            try
            {
                var password = _authTokenRegex.Match(commandLine).Groups[1].Value;
                var port = _portRegex.Match(commandLine).Groups[1].Value;

                return new LeagueConnectionSettings(int.Parse(port), password);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error while trying to get the status for LeagueClientUx: {e}\n\n(CommandLine = {commandLine})");
            }
        }

        return null;
    }
}