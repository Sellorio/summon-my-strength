using System;

namespace SummonMyStrength.Api.Connectors.WebSocket;

internal class LeagueMessageHandler
{
    public object Owner { get; set; }
    public MessageId MessageId { get; set; }
    public MessageAction Actions { get; set; }
    public Type PayloadType { get; set; }
    public Delegate Handler { get; set; }
}
