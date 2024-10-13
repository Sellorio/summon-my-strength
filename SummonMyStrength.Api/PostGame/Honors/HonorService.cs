using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.Connectors.WebSocket;
using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PostGame.Honors;

internal class HonorService : IHonorService, IDisposable
{
    // the api ignores honor requests if they come too soon after the
    // honor ballot is created
    private const int HonorBallotCreateHonorCooldown = 1500;

    // the api ignores honor requests if they come too soon off the heels
    // of another honor request
    private const int HonorPlayerCooldownMilliseconds = 2000;

    private readonly ILeagueClientWebSocketConnector _leagueClientWebSocketConnector;
    private readonly ILeagueClientApiConnector _leagueClientApiConnector;

    private DateTime? _ballotCreated;
    private DateTime? _lastSentHonorRequest;

    public event Func<HonorBallot, Task> HonorBallotCreated;
    public event Func<HonorBallot, Task> HonorBallotUpdated;

    public HonorService(ILeagueClientWebSocketConnector leagueClientWebSocketConnector, ILeagueClientApiConnector leagueClientApiConnector)
    {
        _leagueClientWebSocketConnector = leagueClientWebSocketConnector;
        _leagueClientApiConnector = leagueClientApiConnector;

        _leagueClientWebSocketConnector.AddMessageHandler<HonorBallot>(
            this,
            MessageId.HonorBallot,
            MessageAction.Create,
            async x =>
            {
                _ballotCreated = DateTime.UtcNow;
                await HonorBallotCreated.InvokeAsync(x);
            });

        _leagueClientWebSocketConnector.AddMessageHandler<HonorBallot>(
            this,
            MessageId.HonorBallot,
            MessageAction.Update,
            x => HonorBallotUpdated.InvokeAsync(x));

        _leagueClientWebSocketConnector.AddMessageHandler<HonorBallot>(
            this,
            MessageId.HonorBallot,
            MessageAction.Delete,
            x =>
            {
                _ballotCreated = null;
                return Task.CompletedTask;
            });
    }

    public async Task HonorPlayerAsync(PlayerHonor honor)
    {
        if (_ballotCreated != null)
        {
            var delta = (int)(DateTime.UtcNow - _ballotCreated.Value).TotalMilliseconds;

            if (delta < HonorBallotCreateHonorCooldown)
            {
                await Task.Delay(HonorBallotCreateHonorCooldown - delta);
            }
        }

        if (_lastSentHonorRequest != null)
        {
            var delta = (int)(DateTime.UtcNow - _lastSentHonorRequest.Value).TotalMilliseconds;

            if (delta < HonorPlayerCooldownMilliseconds)
            {
                await Task.Delay(HonorPlayerCooldownMilliseconds - delta);
            }
        }

        await _leagueClientApiConnector.PostAsync(
            "lol-honor-v2/v1/honor-player",
            honor);

        await _leagueClientApiConnector.PostAsync(
            "lol-honor/v1/honor",
            new { recipientPuuid = honor.Puuid, honorType = honor.HonorType });

        _lastSentHonorRequest = DateTime.UtcNow;
    }

    public void Dispose()
    {
        _leagueClientWebSocketConnector.RemoveAllMessageHandlers(this);
    }
}
