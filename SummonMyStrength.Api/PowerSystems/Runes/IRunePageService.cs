using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PowerSystems.Runes;

public interface IRunePageService
{
    event Func<RunePage[], Task> RunePagesUpdated;
    event Func<RunePage, Task> RunePageUpdated;

    Task<RunePage> CreatePageAsync(RunePage newPage);
    Task DeleteAllPagesAsync();
    Task DeletePageAsync(int id);
    Task<RunePage> GetCurrentPageAsync();
    Task<RunePage[]> GetPagesAsync();
    Task SetCurrentPageAsync(int id);
}
