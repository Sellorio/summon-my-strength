using SummonMyStrength.Api.Items;
using System.Collections.Generic;

namespace SummonMyStrength.Simulation.Items;

public class SimulationItem
{
    public Item Item { get; set; }
    public Dictionary<ItemHook, string> Hooks { get; set; }
}
