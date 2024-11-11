using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect;

public interface IChampSelectBenchService
{
    Task SwapWithBenchAsync(int championId);
}
