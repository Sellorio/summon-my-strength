using SummonMyStrength.Simulation.Results;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SummonMyStrength.Simulation;

public class SimulationResult(ImmutableList<SimulationDamageGraph> damageGraphs, ImmutableArray<ActionGraphNode> actionGraph)
{
    public IReadOnlyList<SimulationDamageGraph> DamageGraphs => damageGraphs;
    public IReadOnlyList<ActionGraphNode> ActionGraph => actionGraph;
}
