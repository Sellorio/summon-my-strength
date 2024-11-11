using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.Connectors.WebSocket;
using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect;

internal class ChampSelectSessionService : IChampSelectSessionService, IDisposable
{
    private readonly ILeagueClientWebSocketConnector _clientWebSocketConnector;
    private readonly ILeagueClientApiConnector _clientApiConnector;

    public event Func<ChampSelectSession, Task> ChampSelectSessionCreated;
    public event Func<ChampSelectSession, Task> ChampSelectSessionUpdated;
    public event Func<ChampSelectSession, Task> ChampSelectSessionDeleted;

    public ChampSelectSessionService(ILeagueClientWebSocketConnector clientWebSocketConnector, ILeagueClientApiConnector clientApiConnector)
    {
        _clientWebSocketConnector = clientWebSocketConnector;
        _clientApiConnector = clientApiConnector;

        _clientWebSocketConnector.AddMessageHandler<ChampSelectSession>(
            this,
            MessageId.ChampSelectSession,
            MessageAction.Create,
            x => ChampSelectSessionCreated.InvokeAsync(x));

        _clientWebSocketConnector.AddMessageHandler<ChampSelectSession>(
            this,
            MessageId.ChampSelectSession,
            MessageAction.Update,
            x => ChampSelectSessionUpdated.InvokeAsync(x));

        _clientWebSocketConnector.AddMessageHandler<ChampSelectSession>(
            this,
            MessageId.ChampSelectSession,
            MessageAction.Delete,
            x => ChampSelectSessionDeleted.InvokeAsync(x));
    }

    public void Dispose()
    {
        _clientWebSocketConnector.RemoveAllMessageHandlers(this);
    }

    public async Task<ChampSelectSession> GetSessionAsync()
    {
        return await _clientApiConnector.GetAsync<ChampSelectSession>("lol-champ-select/v1/session");
    }
}
