using System.Threading.Tasks;

namespace SummonMyStrength.Api.Collections.Champions;

public interface IChampionService
{
    Task<Champion[]> GetChampionsAsync();
    string GetIconUrl(Champion champion);
}
