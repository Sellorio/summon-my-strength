using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect;

public interface IChampSelectPickOrderSwapService
{
    Task AcceptSwapAsync(long id);
    Task DeclineSwapAsync(long id);
}
