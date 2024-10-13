using System;

namespace SummonMyStrength.Api.Connectors.WebSocket;

[Flags]
public enum MessageAction
{
    None = 0,
    Create = 1,
    Update = 2,
    Delete = 4,
    Get = 8,
    UnregisteredMessageAction = 16,
    All = Create | Update | Delete | Get | UnregisteredMessageAction,
}
