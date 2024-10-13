namespace SummonMyStrength.Api.Connectors.WebSocket;

internal static class LeagueMessageHelper
{
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
        return messagePath switch
        {
            "/lol-gameflow/v1/gameflow-phase" => MessageId.GameflowPhase,
            "/lol-honor-v2/v1/ballot" => MessageId.HonorBallot,
            _ => MessageId.UnregisteredMessageId
        };
    }
}
