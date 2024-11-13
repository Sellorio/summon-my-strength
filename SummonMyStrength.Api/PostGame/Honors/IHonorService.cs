using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PostGame.Honors;

public interface IHonorService
{
    event Func<HonorBallot, Task> HonorPhaseStarted;
    event Func<HonorBallot, Task> HonorBallotUpdated;

    Task<HonorBallot> GetHonorBallotAsync();
    Task HonorPlayerAsync(PlayerHonor honor);
}
