using System.Text.Json.Serialization;

namespace SummonMyStrength.Maui.Services.LiveApi;

public class GameEvents
{
    [JsonPropertyName("Events")]
    public IList<GameEvent> Events { get; set; }
}
