using SummonMyStrength.Api.ChampSelect.Trades;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect;

public interface IChampSelectChampionTradeService
{
    Task AcceptTradeAsync(long id);
    Task DeclineTradeAsync(long id);
    Task<OngoingTrade> GetOngoingTradeAsync();
    Task<ChampSelectTradeContract> RequestTradeAsync(long id);
}
