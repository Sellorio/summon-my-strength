using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect;

public interface IChampSelectSessionService
{
    event Func<ChampSelectSession, Task> ChampSelectSessionCreated;
    event Func<ChampSelectSession, Task> ChampSelectSessionUpdated;
    event Func<ChampSelectSession, Task> ChampSelectSessionDeleted;

    Task<ChampSelectSession> GetSessionAsync();
}
