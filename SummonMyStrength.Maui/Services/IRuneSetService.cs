using SummonMyStrength.Api.Champions;

namespace SummonMyStrength.Maui.Services
{
    public interface IRuneSetService
    {
        bool HasUnsavedChanges { get; }
        public event Func<Task> HasUnsavedChangesChanged;
        Task LoadRunesForChampionAsync(Champion champion);
        Task SaveRunes();
    }
}
