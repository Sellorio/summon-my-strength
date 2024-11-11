using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.Connectors.WebSocket;
using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Matchmaking;

internal class ReadyCheckService : IReadyCheckService, IDisposable
{
    private readonly ILeagueClientWebSocketConnector _clientWebSocketConnector;
    private readonly ILeagueClientApiConnector _clientApiConnector;

    private ReadyCheck _activeReadyCheck;

    public event Func<ReadyCheck, Task> ReadyCheckCreated;
    public event Func<ReadyCheck, Task> ReadyCheckUpdated;
    public event Func<ReadyCheck, Task> ReadyCheckDeleted;

    public ReadyCheckService(ILeagueClientWebSocketConnector clientWebSocketConnector, ILeagueClientApiConnector clientApiConnector)
    {
        _clientWebSocketConnector = clientWebSocketConnector;
        _clientApiConnector = clientApiConnector;

        _clientWebSocketConnector.AddMessageHandler<ReadyCheck>(
            this,
            MessageId.ReadyCheck,
            MessageAction.Update,
            async x =>
            {
                if (x.State is ReadyCheckState.EveryoneReady or ReadyCheckState.Error) // ready check ended
                {
                    if (_activeReadyCheck != null)
                    {
                        var previousReadyCheck = _activeReadyCheck;
                        _activeReadyCheck = null;
                        await ReadyCheckDeleted.InvokeAsync(previousReadyCheck);
                    }
                }
                else if (_activeReadyCheck == null)
                {
                    _activeReadyCheck = x;
                    await ReadyCheckCreated.InvokeAsync(_activeReadyCheck);
                }
                else
                {
                    _activeReadyCheck.State = x.State;
                    _activeReadyCheck.DeclinerIds = x.DeclinerIds;
                    _activeReadyCheck.SuppressUx = x.SuppressUx;
                    _activeReadyCheck.PlayerResponse = x.PlayerResponse;
                    _activeReadyCheck.DodgeWarning = x.DodgeWarning;
                    _activeReadyCheck.Timer = x.Timer;
                    await ReadyCheckUpdated.InvokeAsync(_activeReadyCheck);
                }
            });
    }

    public void Dispose()
    {
        _clientWebSocketConnector.RemoveAllMessageHandlers(this);
    }

    public async Task<ReadyCheck> GetReadyCheckAsync()
    {
        return _activeReadyCheck ??= await _clientApiConnector.GetAsync<ReadyCheck>("lol-matchmaking/v1/ready-check");
    }

    public async Task AcceptReadyCheckAsync()
    {
        if (_activeReadyCheck != null)
        {
            await _clientApiConnector.PostAsync("lol-matchmaking/v1/ready-check/accept", null);
        }
    }

    public async Task DeclineReadyCheckAsync()
    {
        if (_activeReadyCheck != null)
        {
            await _clientApiConnector.PostAsync("lol-matchmaking/v1/ready-check/decline", null);
        }
    }
}
