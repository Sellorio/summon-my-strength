using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.Connectors.WebSocket;
using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PostGame.Stats;

internal class PostGameStatsService : IPostGameStatsService, IDisposable
{
    private readonly ILeagueClientWebSocketConnector _clientWebSocketConnector;

    public event Func<PostGameStats, Task> PostGameStatsCreated;

    public PostGameStatsService(ILeagueClientWebSocketConnector clientWebSocketConnector)
    {
        _clientWebSocketConnector = clientWebSocketConnector;

        _clientWebSocketConnector.AddMessageHandler<PostGameStats>(
            this,
            MessageId.EndOfGameStats,
            MessageAction.Create,
            x => PostGameStatsCreated.InvokeAsync(x));
    }

    public void Dispose()
    {
        _clientWebSocketConnector.RemoveAllMessageHandlers(this);
    }
}
