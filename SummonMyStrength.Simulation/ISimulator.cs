using System.Threading.Tasks;

namespace SummonMyStrength.Simulation;

public interface ISimulator
{
    Task<SimulationResult> SimulateAsync(SimulationParameters parameters);
}
