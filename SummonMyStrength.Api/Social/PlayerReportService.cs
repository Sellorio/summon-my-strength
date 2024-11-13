using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.Connectors.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Social;

internal class PlayerReportService : IPlayerReportService
{
    private readonly ILeagueClientWebSocketConnector _clientWebSocketConnector;

    public event Func<Dictionary<string, InGameReportStatus>, Task> InGameReportStatusUpdated;

    public PlayerReportService(ILeagueClientWebSocketConnector clientWebSocketConnector)
    {
        _clientWebSocketConnector = clientWebSocketConnector;

        _clientWebSocketConnector.AddMessageHandler<Dictionary<string, InGameReportStatus>>(
            this,
            MessageId.PlayerMutes,
            MessageAction.Update,
            x => InGameReportStatusUpdated.InvokeAsync(x));
    }
}
