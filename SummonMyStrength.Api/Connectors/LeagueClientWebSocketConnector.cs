using System.Diagnostics;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using SummonMyStrength.Api.Connectors.WebSocket;
using System.Linq;
using System.IO;

namespace SummonMyStrength.Api.Connectors;

internal class LeagueClientWebSocketConnector : ILeagueClientWebSocketConnector
{
    private static readonly JsonSerializerOptions _jsonOptions;

    static LeagueClientWebSocketConnector()
    {
        _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    private readonly ILeagueConnectionSettingsProvider _connectionSettingsProvider;
    private readonly ILeagueClientApiConnector _clientApi;
    private readonly List<LeagueMessageHandler> _messageHandlers = new();
    private ClientWebSocket _socket;
    private CancellationTokenSource _cancellationTokenSource;

    public bool IsConnected { get; private set; }

    public event Func<Task> Disconnected;
    public event Func<Task> Connected;
    public event Func<Exception, Task> MessageHandlerFailure;

    public LeagueClientWebSocketConnector(ILeagueConnectionSettingsProvider connectionSettingsProvider, ILeagueClientApiConnector clientApi)
    {
        _connectionSettingsProvider = connectionSettingsProvider;
        _clientApi = clientApi;
    }

    public async Task<bool> WaitForConnectionAsync()
    {
        // only exits once successfully connected
        await InternalConnectAsync(CancellationToken.None);

        return true;
    }

    public void AddMessageHandler<TPayload>(
        object owner,
        MessageId messageId,
        MessageAction action,
        Func<TPayload, Task> handler)
    {
        _messageHandlers.Add(new LeagueMessageHandler
        {
            Owner = owner,
            MessageId = messageId,
            Actions = action,
            Handler = handler,
            PayloadType = typeof(TPayload)
        });
    }

    public void RemoveAllMessageHandlers(object owner)
    {
        foreach (var handler in _messageHandlers.Where(x => x.Owner == owner).ToList())
        {
            _messageHandlers.Remove(handler);
        }
    }

    public void Dispose()
    {
        ((IDisposable)_socket)?.Dispose();
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

    private async Task TryConnectAsync()
    {
        if (IsConnected)
        {
            return;
        }

        LeagueConnectionSettings settings = null;

        if (!await _connectionSettingsProvider.TryReadAsync(x => settings = x))
        {
            return;
        }

        try
        {
            var socketConnection = _socket = new ClientWebSocket();
            _socket.Options.Credentials = new NetworkCredential("riot", settings.Password);
            _socket.Options.RemoteCertificateValidationCallback = (a, b, c, d) => true;
            _socket.Options.AddSubProtocol("wamp");

            // cancel this token if we add a Disconnect method to this class
            _cancellationTokenSource = new CancellationTokenSource();

            await _socket.ConnectAsync(new Uri("wss://127.0.0.1:" + settings.PortNumber + "/"), CancellationToken.None);

            _ = Task.Factory.StartNew(
            () => ReceiveWebSocketMessagesAsync(socketConnection, _cancellationTokenSource.Token),
            CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            await _socket.SendAsync(Encoding.UTF8.GetBytes("[5, \"OnJsonApiEvent\"]"), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);

            IsConnected = true;
            await Connected.InvokeAsync();
        }
        catch (WebSocketException ex) when (ex.Message.Contains("Unable to connect"))
        {
            _socket = null;
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
        _socket = null;

        await Disconnected?.InvokeAsync();

        await Task.Delay(2000, cancellationToken);
        await InternalConnectAsync(cancellationToken);
    }

    private async Task HandleMessageAsync(string content)
    {
        var rawMessage = JsonSerializer.Deserialize<JsonElement>(content, _jsonOptions);

        if (rawMessage.ValueKind != JsonValueKind.Array
            || rawMessage.GetArrayLength() != 3
            || rawMessage[0].ValueKind != JsonValueKind.Number
            || rawMessage[0].GetInt64() != 8
            || rawMessage[1].ValueKind != JsonValueKind.String
            || rawMessage[1].GetString() != "OnJsonApiEvent")
        {
            return;
        }

        var eventData = rawMessage[2];
        var uri = eventData.GetProperty("uri").GetString();
        var eventType = eventData.GetProperty("eventType").GetString();
        var eventBody = eventData.GetProperty("data");

        var messageId = LeagueMessageHelper.ParseMessageId(uri);
        var messageAction = LeagueMessageHelper.ParseAction(eventType);
        var deserializedPayloadInstances = new Dictionary<Type, object>();

        foreach (var handler in _messageHandlers.Where(x => x.MessageId == messageId && (x.Actions & messageAction) != 0))
        {
            if (!deserializedPayloadInstances.TryGetValue(handler.PayloadType, out var messagePayload))
            {
                messagePayload = JsonSerializer.Deserialize(eventBody, handler.PayloadType, _jsonOptions);
                deserializedPayloadInstances.Add(handler.PayloadType, messagePayload);
            }

            try
            {
                var handlerTask = (Task)handler.Handler.DynamicInvoke(messagePayload);
                await handlerTask;
            }
            catch (Exception ex)
            {
                if (MessageHandlerFailure != null)
                {
                    await MessageHandlerFailure.InvokeAsync(ex);
                }
            }
        }
    }
}
