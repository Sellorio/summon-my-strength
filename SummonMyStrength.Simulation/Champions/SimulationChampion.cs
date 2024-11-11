using SummonMyStrength.Api.Collections.Champions;
using System.Collections.Generic;

namespace SummonMyStrength.Simulation.Champions;

public class SimulationChampion
{
    public Champion Champion { get; set; }
    public Dictionary<ChampionHook, string> Hooks { get; set; }
}
