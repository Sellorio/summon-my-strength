using SummonMyStrength.Maui.Services.LiveApi;

namespace SummonMyStrength.Maui.Services;

public interface IGameInfoAccessor
{
    event Func<Task> GameInfoChanged;
    GameInfo GameInfo { get; }
    bool SaveStatisticsOnEnd { get; set; }
}
