using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PostGame.Stats;

public interface IPostGameStatsService
{
    event Func<Task> EnteredPostGameStatsPhase;
    event Func<PostGameStats, Task> PostGameStatsCreated;
}
