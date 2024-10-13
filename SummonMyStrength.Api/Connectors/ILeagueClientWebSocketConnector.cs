using SummonMyStrength.Api.Connectors.WebSocket;
using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Connectors;

public interface ILeagueClientWebSocketConnector
{
    bool IsConnected { get; }

    event Func<Task> Connected;
    event Func<Task> Disconnected;

    Task<bool> WaitForConnectionAsync();
    void AddMessageHandler<TPayload>(object owner, MessageId messageId, MessageAction action, Func<TPayload, Task> handler);
    void RemoveAllMessageHandlers(object owner);
}
