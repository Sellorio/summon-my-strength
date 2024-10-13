using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.Connectors.WebSocket;
using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.General;

internal class GameflowService : IGameflowService, IDisposable
{
    private readonly ILeagueClientApiConnector _clientApiConnector;
    private readonly ILeagueClientWebSocketConnector _clientWebSocketConnector;

    public event Func<GameflowPhase, Task> PhaseChanged;

    public GameflowService(ILeagueClientApiConnector clientApiConnector, ILeagueClientWebSocketConnector clientWebSocketConnector)
    {
        _clientApiConnector = clientApiConnector;
        _clientWebSocketConnector = clientWebSocketConnector;

        _clientWebSocketConnector.AddMessageHandler<GameflowPhase>(
            this,
            MessageId.GameflowPhase,
            MessageAction.Create | MessageAction.Update,
            x => PhaseChanged.InvokeAsync(x));
    }

    public async Task<GameflowPhase> GetPhaseAsync()
    {
        return await _clientApiConnector.GetAsync<GameflowPhase>("lol-gameflow/v1/gameflow-phase");
    }

    public void Dispose()
    {
        _clientWebSocketConnector.RemoveAllMessageHandlers(this);
    }
}
