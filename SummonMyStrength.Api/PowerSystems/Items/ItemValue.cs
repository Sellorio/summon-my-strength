using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.PowerSystems.Items;

public class ItemValue
{
    [JsonPropertyName("base")]
    public int CombineCost { get; set; }

    [JsonPropertyName("total")]
    public int TotalCost { get; set; }

    [JsonPropertyName("sell")]
    public int SellValue { get; set; }

    public bool Purchaseable { get; set; }
}
