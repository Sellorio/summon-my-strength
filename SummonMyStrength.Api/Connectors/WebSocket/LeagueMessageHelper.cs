using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SummonMyStrength.Api.Connectors.WebSocket;

internal static class LeagueMessageHelper
{
    private static readonly List<(MessagePathAttribute MessagePathMatcher, MessageId MessageId)> _messageIdMatchers;

    static LeagueMessageHelper()
    {
        _messageIdMatchers =
            typeof(MessageId)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(x => (x.GetCustomAttribute<MessagePathAttribute>(), (MessageId)x.GetValue(null)))
                .Where(x => x.Item1 != null)
                .ToList();
    }

    internal static MessageAction ParseAction(string messageAction)
    {
        return messageAction switch
        {
            "Create" => MessageAction.Create,
            "Update" => MessageAction.Update,
            "Delete" => MessageAction.Delete,
            "Get" => MessageAction.Get,
            _ => MessageAction.UnregisteredMessageAction
        };
    }

    internal static MessageId ParseMessageId(string messagePath)
    {
        foreach (var messageIdMatcher in _messageIdMatchers)
        {
            if (messageIdMatcher.MessagePathMatcher.Path == messagePath ||
                messageIdMatcher.MessagePathMatcher.StartsWith && messagePath.StartsWith(messageIdMatcher.MessagePathMatcher.Path))
            {
                return messageIdMatcher.MessageId;
            }
        }

        return MessageId.UnregisteredMessageId;
    }
}
