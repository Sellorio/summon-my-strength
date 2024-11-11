using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Matchmaking;

public interface IReadyCheckService
{
    event Func<ReadyCheck, Task> ReadyCheckCreated;
    event Func<ReadyCheck, Task> ReadyCheckUpdated;
    event Func<ReadyCheck, Task> ReadyCheckDeleted;

    Task AcceptReadyCheckAsync();
    Task DeclineReadyCheckAsync();
    Task<ReadyCheck> GetReadyCheckAsync();
}
