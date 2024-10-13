using SummonMyStrength.Simulation.Data;
using SummonMyStrength.Simulation.Scripts;
using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Simulation;

internal class Simulator(IScriptExecutor scriptExecutor) : ISimulator
{
    public async Task<SimulationResult> SimulateAsync(SimulationParameters parameters)
    {
        var state = new SimulationState();

        foreach (var action in parameters.Actions)
        {
            await PerformActionAsync(state, action);
        }

        throw new NotImplementedException();
    }

    private async Task PerformActionAsync(SimulationState state, SimulationAction action)
    {
        throw new NotImplementedException();
    }
}
