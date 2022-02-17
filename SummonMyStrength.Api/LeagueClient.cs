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

namespace SummonMyStrength.Api
{
    public sealed class LeagueClient : IDisposable
    {
        private static readonly Regex _authTokenRegex = new("\"--remoting-auth-token=(.+?)\"");
        private static readonly Regex _portRegex = new("\"--app-port=(\\d+?)\"");
        private static readonly JsonSerializerOptions _jsonSerializerOptions;

        private static readonly HttpMessageHandler _httpMessageHandler;

        private readonly List<Action<LeagueClientWebSocketMessage>> _webSocketMessageHandlers = new();
        private ClientWebSocket _socketConnection;
        private CancellationTokenSource _cancellationTokenSource;

        public HttpClient HttpClient { get; private set; }

        internal HttpClient DataDragonHttpClient { get; set; }

        internal JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions;

        public bool IsConnected => _socketConnection?.State == WebSocketState.Open;

        public ChampionsModule Champions { get; }
        public ChampSelectModule ChampSelect { get; }
        public GameflowModule Gameflow { get; }
        public MatchmakingModule Matchmaking { get; }
        public PerksModule Perks { get; }

        static LeagueClient()
        {
            _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            _httpMessageHandler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            };
        }

        public LeagueClient()
        {
            DataDragonHttpClient = new HttpClient(_httpMessageHandler);
            DataDragonHttpClient.BaseAddress = new Uri("http://ddragon.leagueoflegends.com/");

            Champions = new ChampionsModule(this);
            ChampSelect = new ChampSelectModule(this);
            Gameflow = new GameflowModule(this);
            Matchmaking = new MatchmakingModule(this);
            Perks = new PerksModule(this);
        }

        public async Task<bool> ConnectAsync()
        {
            await InternalConnectAsync(CancellationToken.None);
            return true;
        }

        public void AddMessageHandler(Action<LeagueClientWebSocketMessage> handler)
        {
            if (!_webSocketMessageHandlers.Contains(handler))
            {
                _webSocketMessageHandlers.Add(handler);
            }
        }

        public void Dispose()
        {
            ((IDisposable)_socketConnection)?.Dispose();
        }

        private async Task InternalConnectAsync(CancellationToken cancellationToken)
        {
            await TryConnectAsync();

            do
            {
                await Task.Delay(2000, cancellationToken);
                await TryConnectAsync();
            }
            while (!IsConnected);
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

                HttpClient = new HttpClient(_httpMessageHandler);
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authenticationToken);
                HttpClient.BaseAddress = new Uri("https://127.0.0.1:" + status.PortNumber);

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
            StringBuilder currentMessage = new StringBuilder(1000);

            while (true)
            {
                var receivedMessage = await webSocket.ReceiveAsync(receiveBuffer, cancellationToken);

                if (receivedMessage.MessageType == WebSocketMessageType.Close)
                {
                    _socketConnection = null;
                    HttpClient = null;

                    await Task.Delay(2000, cancellationToken);
                    await InternalConnectAsync(cancellationToken);

                    return;
                }
                else if (receivedMessage.MessageType == WebSocketMessageType.Text)
                {
                    currentMessage.Append(Encoding.UTF8.GetString(receiveBuffer.Array, 0, receivedMessage.Count));

                    if (receivedMessage.EndOfMessage)
                    {
                        try
                        {
                            HandleMessage(currentMessage.ToString());
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("Error when handling message:\r\n" + ex);
                        }

                        currentMessage = new StringBuilder(1000);
                    }
                }
            }
        }

        private void HandleMessage(string content)
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
            var webSocketMessage =
                new LeagueClientWebSocketMessage(
                    eventData.GetProperty("uri").GetString(),
                    eventData.GetProperty("eventType").GetString(),
                    eventData.GetProperty("data"));

            _webSocketMessageHandlers.ForEach(x => x.Invoke(webSocketMessage));
        }

        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
        private static LeagueClientConnectionSettings GetConnectionSettingsFromLeagueClientProcess()
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

                    return new LeagueClientConnectionSettings(int.Parse(port), password);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Error while trying to get the status for LeagueClientUx: {e}\n\n(CommandLine = {commandLine})");
                }
            }

            return null;
        }
    }
}