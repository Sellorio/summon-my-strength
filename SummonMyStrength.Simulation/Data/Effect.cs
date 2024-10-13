using System.Text.Json.Serialization;

namespace SummonMyStrength.Simulation.Data;

public class Effect
{
    /// <summary>
    /// When duration or frequency are set, start time must be provided.
    /// </summary>
    [JsonPropertyName("startTime")]
    public int? StartTime { get; set; }

    /// <summary>
    /// The duration of the effect in milliseonds. If null, is either a once-off or a permanent effect.
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    /// <summary>
    /// The frequency of repetitions (e.g. of damage) in milliseconds.
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Frequency { get; set; }

    /// <summary>
    /// A name for the effect, useful in scripts to identify effects that need future adjustment (e.g. stacking buffs that refresh durations).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EffectType? Type { get; set; }

    [JsonPropertyName("value")]
    public int Value { get; set; }
}
