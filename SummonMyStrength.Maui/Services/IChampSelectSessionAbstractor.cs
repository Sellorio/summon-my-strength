using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Maui.Services.ChampSelect;

namespace SummonMyStrength.Maui.Services;

public interface IChampSelectSessionAbstractor
{
    ChampSelectPhase Phase { get; }
    int? SelectedChampionId { get; }

    event Func<ChampSelectPhase, ChampSelectPhase, Task> OnPhaseChanged;
    event Func<int?, int?, Task> OnSelectedChampionIdChanged;

    Task ApplyChangesAsync(ChampSelectSession session);
}
