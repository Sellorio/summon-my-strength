using System.Text.Json.Serialization;

namespace SummonMyStrength.Maui.Services.LiveApi
{
    public class PlayerItem
    {
        public bool CanUse { get; set; }
        public bool Consumable { get; set; }
        public int Count { get; set; }
        public string DisplayName { get; set; }
        [JsonPropertyName("itemID")]
        public int ItemId { get; set; }
        public int Price { get; set; }
        public string RawDescription { get; set; }
        public string RawDisplayName { get; set; }
        public int Slot { get; set; }
    }
}
