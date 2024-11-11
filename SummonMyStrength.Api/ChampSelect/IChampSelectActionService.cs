using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect;

public interface IChampSelectActionService
{
    Task PatchActionAsync(long id, ChampSelectAction action);
}
