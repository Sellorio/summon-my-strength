using SummonMyStrength.Simulation.Champions;
using SummonMyStrength.Simulation.Items;
using System.Collections.Generic;

namespace SummonMyStrength.Simulation;

public class SimulationParameters
{
    public SimulationChampion Champion { get; set; }
    public List<SimulationItem> Items { get; set; }
    public List<SimulationTarget> Targets { get; set; }
    public List<SimulationAction> Actions { get; set; }
}
