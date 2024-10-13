using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SummonMyStrength.Simulation.Data;

public class SimulationState
{
    [JsonPropertyName("time")]
    public int Time { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("variables")]
    public Dictionary<string, JsonElement> Variables { get; set; }

    [JsonPropertyName("effects")]
    public Dictionary<string, Effect> Effects { get; set; }

    [JsonPropertyName("timers")]
    public Dictionary<string, int?> Timers { get; set; }
}
