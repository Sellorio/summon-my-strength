using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.PowerSystems.SummonerSpells;

public class SummonerSpell
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("key")]
    public string IdString
    {
        get => Id.ToString();
        set => Id = int.Parse(value);
    }

    [JsonPropertyName("id")]
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] Modes { get; set; }
}
