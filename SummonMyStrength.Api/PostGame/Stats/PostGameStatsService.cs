using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.Connectors.WebSocket;
using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PostGame.Stats;

internal class PostGameStatsService : IPostGameStatsService, IDisposable
{
    private readonly ILeagueClientWebSocketConnector _clientWebSocketConnector;

    public event Func<Task> EnteredPostGameStatsPhase;
    public event Func<PostGameStats, Task> PostGameStatsCreated;

    public PostGameStatsService(ILeagueClientWebSocketConnector clientWebSocketConnector)
    {
        _clientWebSocketConnector = clientWebSocketConnector;

        _clientWebSocketConnector.AddMessageHandler<PreEndOfGameSequenceMessageBody>(
            this,
            MessageId.PreEndOfGameSequenceEvent,
            MessageAction.Update,
            async msg =>
            {
                if (msg.Name == "") // blank means all phases are over and stats screen is shown
                {
                    await EnteredPostGameStatsPhase.InvokeAsync();
                }
            });

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
