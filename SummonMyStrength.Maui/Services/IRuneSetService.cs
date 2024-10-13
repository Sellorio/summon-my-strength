using SummonMyStrength.Api.Champions;

namespace SummonMyStrength.Maui.Services;

public interface IRuneSetService
{
    Champion RunesLoadedFor { get; }
    public event Func<Task> RunesLoadedForChanged;
    Task LoadRunesForChampionAsync(Champion champion);
}
