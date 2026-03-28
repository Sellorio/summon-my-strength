using SummonMyStrength.Api.Connectors.DataDragon;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.PowerSystems.Items;

public class Item
{
    public string Id { get; set; }

    public string Name { get; set; }

    public ImageReference Image { get; set; }

    [JsonPropertyName("plaintext")]
    public string ShortDescription { get; set; }

    [JsonPropertyName("into")]
    public string[] BuildsInto { get; set; }

    [JsonPropertyName("gold")]
    public ItemValue ItemValue { get; set; }

    public string[] Tags { get; set; }

    [JsonPropertyName("maps")]
    public Dictionary<int, bool> EnabledMaps { get; set; }
}
