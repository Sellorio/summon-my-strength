using SummonMyStrength.Api.PostGame;

namespace SummonMyStrength.Maui.Services;

public interface IPostGameStatsService
{
    PostGameStats PostGameStats { get; }
    (string Key, string Name, Func<PostGamePlayerStats, decimal> reader)?[] StatReaders { get; }
    event Func<Task> PostGameStatsChanged;
}
