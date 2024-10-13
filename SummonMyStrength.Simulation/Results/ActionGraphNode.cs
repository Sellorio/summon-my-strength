namespace SummonMyStrength.Simulation.Results;

public class ActionGraphNode(SimulationAction action, int time)
{
    public SimulationAction Action => action;
    public int Time => time;
}
