using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.Connectors.WebSocket;
using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PostGame.Honors;

internal class HonorService : IHonorService, IDisposable
{
    // the api ignores honor requests if they come too soon after the
    // honor ballot is created
    private const int HonorBallotCreateHonorCooldown = 1000;

    // the api ignores honor requests if they come too soon off the heels
    // of another honor request
    private const int HonorPlayerCooldownMilliseconds = 100;

    private readonly ILeagueClientWebSocketConnector _leagueClientWebSocketConnector;
    private readonly ILeagueClientApiConnector _leagueClientApiConnector;

    private string _postGameSequenceStage;
    private HonorBallot _honorBallot;
    private DateTime? _startedHonorPhase;
    private DateTime? _lastSentHonorRequest;

    public event Func<HonorBallot, Task> HonorPhaseStarted;
    public event Func<HonorBallot, Task> HonorBallotUpdated;

    public HonorService(ILeagueClientWebSocketConnector leagueClientWebSocketConnector, ILeagueClientApiConnector leagueClientApiConnector)
    {
        _leagueClientWebSocketConnector = leagueClientWebSocketConnector;
        _leagueClientApiConnector = leagueClientApiConnector;

        _leagueClientWebSocketConnector.AddMessageHandler<PreEndOfGameSequenceMessageBody>(
            this,
            MessageId.PreEndOfGameSequenceEvent,
            MessageAction.Create | MessageAction.Update,
            async msg =>
            {
                _postGameSequenceStage = msg.Name;

                if (_postGameSequenceStage == "honor-vote" && _honorBallot != null)
                {
                    _startedHonorPhase = DateTime.UtcNow;
                    await HonorPhaseStarted.Invoke(_honorBallot);
                }
            });

        _leagueClientWebSocketConnector.AddMessageHandler<HonorBallot>(
            this,
            MessageId.HonorBallot,
            MessageAction.Create,
            async newHonorBallot =>
            {
                _honorBallot = newHonorBallot;

                if (_postGameSequenceStage == "honor-vote" && _honorBallot != null)
                {
                    _startedHonorPhase = DateTime.UtcNow;
                    await HonorPhaseStarted.InvokeAsync(newHonorBallot);
                }
            });

        _leagueClientWebSocketConnector.AddMessageHandler<HonorBallot>(
            this,
            MessageId.HonorBallot,
            MessageAction.Update,
            async updatedHonorBallot =>
            {
                if (_postGameSequenceStage == "honor-vote" && _honorBallot != null)
                {
                    _honorBallot = updatedHonorBallot;
                    await HonorBallotUpdated.InvokeAsync(updatedHonorBallot);
                }
            });

        _leagueClientWebSocketConnector.AddMessageHandler<HonorBallot>(
            this,
            MessageId.HonorBallot,
            MessageAction.Delete,
            x =>
            {
                _honorBallot = null;
                _startedHonorPhase = null;
                return Task.CompletedTask;
            });
    }

    public async Task<HonorBallot> GetHonorBallotAsync()
    {
        return await _leagueClientApiConnector.GetAsync<HonorBallot>("lol-honor-v2/v1/ballot");
    }

    public async Task HonorPlayerAsync(PlayerHonor honor)
    {
        if (_startedHonorPhase == null)
        {
            return;
        }

        var delta = (int)(DateTime.UtcNow - _startedHonorPhase.Value).TotalMilliseconds;

        if (delta < HonorBallotCreateHonorCooldown)
        {
            await Task.Delay(HonorBallotCreateHonorCooldown - delta);
        }

        if (_lastSentHonorRequest != null)
        {
            delta = (int)(DateTime.UtcNow - _lastSentHonorRequest.Value).TotalMilliseconds;

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
