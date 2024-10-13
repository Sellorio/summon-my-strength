using System.Collections.Generic;
using System.Collections.Immutable;

namespace SummonMyStrength.Simulation.Results;

public class SimulationDamageGraph(
    SimulationTarget target,
    ImmutableArray<DamageGraphNode> physicalDamageGraph,
    ImmutableArray<DamageGraphNode> magicDamageGraph,
    ImmutableArray<DamageGraphNode> trueDamageGraph)
{
    public SimulationTarget Target => target;
    public IReadOnlyList<DamageGraphNode> PhysicalDamageGraph => physicalDamageGraph;
    public IReadOnlyList<DamageGraphNode> MagicDamageGraph => magicDamageGraph;
    public IReadOnlyList<DamageGraphNode> TrueDamageGraph => trueDamageGraph;
}
