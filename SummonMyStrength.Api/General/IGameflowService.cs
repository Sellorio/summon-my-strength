using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.General;

public interface IGameflowService
{
    event Func<GameflowPhase, Task> PhaseChanged;

    Task<GameflowPhase> GetPhaseAsync();
}
