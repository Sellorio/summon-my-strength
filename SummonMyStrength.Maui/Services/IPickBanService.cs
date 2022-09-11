using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.ChampSelect;

namespace SummonMyStrength.Maui.Services
{
    public interface IPickBanService
    {
        ChampSelectAssignedPosition? Position { get; }
        event Func<Task> AssignedPositionChanged;
        Task SetAssignedPosition(ChampSelectAssignedPosition position);
        Task<List<Champion>> GetRecentPicksAsync();
        Task<List<Champion>> GetRecentBansAsync();
        Task PickChampionAsync(Champion champion);
        Task BanChampionAsync(Champion champion);
    }
}
