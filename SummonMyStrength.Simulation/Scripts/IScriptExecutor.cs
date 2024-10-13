using SummonMyStrength.Simulation.Data;
using System.Threading.Tasks;

namespace SummonMyStrength.Simulation.Scripts;

public interface IScriptExecutor
{
    Task<SimulationState> InvokeJavaScriptHookAsync(string javascript, SimulationState state);
}
