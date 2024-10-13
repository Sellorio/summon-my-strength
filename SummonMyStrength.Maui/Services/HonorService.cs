using SummonMyStrength.Api;
using SummonMyStrength.Api.General;
using SummonMyStrength.Api.PostGame.Honors;

// /lol-pre-end-of-game/v1/currentSequenceEvent
// Update
// {"name":"honor-vote","priority":1}

namespace SummonMyStrength.Maui.Services;

internal class HonorService : IHonorService
{
    private HonorBallot _honorBallot;
    private string _preEndOfGamePhase;

    private readonly LeagueClient _leagueClient;

    public string PrimaryHonorSummonerName { get; set; }
    public string SecondaryHonorSummonerName { get; set; }

    public HonorService(LeagueClient leagueClient)
    {
        _leagueClient = leagueClient;

        _leagueClient.Gameflow.GameflowPhaseChanged += newPhase =>
        {
            if (newPhase == GameflowPhase.InProgress)
            {
                PrimaryHonorSummonerName = null;
                SecondaryHonorSummonerName = null;
            }

            return Task.CompletedTask;
        };

        _leagueClient.Honors.HonorBallotCreated += OnBallotCreated;
        _leagueClient.PostGame.PreEndOfGamePhaseChanged += PostGame_PreEndOfGamePhaseChanged;
    }

    private async Task PostGame_PreEndOfGamePhaseChanged(string phase)
    {
        if (_preEndOfGamePhase == "honor-vote" && phase != _preEndOfGamePhase)
        {
            _honorBallot = null;
        }

        _preEndOfGamePhase = phase;
        
        if (_preEndOfGamePhase == "honor-vote" && _honorBallot != null)
        {
            await ApplyHonorsAsync();
        }
    }

    private async Task OnBallotCreated(HonorBallot ballot)
    {
        _honorBallot = ballot;

        if (_preEndOfGamePhase == "honor-vote")
        {
            await ApplyHonorsAsync();
        }
    }

    private async Task ApplyHonorsAsync()
    {
        if (PrimaryHonorSummonerName != null)
        {
            await Task.Delay(2000);

            if (_honorBallot.VotePool.Votes < 1)
            {
                return;
            }

            var trimmedSummonerName =
                PrimaryHonorSummonerName.Substring(
                    0,
                    PrimaryHonorSummonerName.IndexOf("#"));

            var primaryHonorCandidate =
                _honorBallot.EligibleAllies.FirstOrDefault(x => x.SummonerName == trimmedSummonerName)
                    ?? _honorBallot.EligibleOpponents.FirstOrDefault(x => x.SummonerName == trimmedSummonerName);

            if (primaryHonorCandidate != null)
            {
                await _leagueClient.Honors.HonorPlayerAsync(
                    new()
                    {
                        GameId = _honorBallot.GameId,
                        Puuid = primaryHonorCandidate.Puuid,
                        SummonerId = primaryHonorCandidate.SummonerId
                    });
            }

            if (_honorBallot.VotePool.Votes < 2 || SecondaryHonorSummonerName == null)
            {
                return;
            }

            trimmedSummonerName =
                SecondaryHonorSummonerName.Substring(
                    0,
                    SecondaryHonorSummonerName.IndexOf("#"));

            var secondaryHonorCandidate =
                _honorBallot.EligibleAllies.FirstOrDefault(x => x.SummonerName == trimmedSummonerName)
                    ?? _honorBallot.EligibleOpponents.FirstOrDefault(x => x.SummonerName == trimmedSummonerName);

            if (secondaryHonorCandidate != null)
            {
                await Task.Delay(2000);

                await _leagueClient.Honors.HonorPlayerAsync(
                    new()
                    {
                        GameId = _honorBallot.GameId,
                        Puuid = secondaryHonorCandidate.Puuid,
                        SummonerId = secondaryHonorCandidate.SummonerId
                    });
            }
        }
    }
}
