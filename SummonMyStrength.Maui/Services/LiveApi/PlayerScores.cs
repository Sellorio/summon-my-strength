using System.Text.Json.Serialization;

namespace SummonMyStrength.Maui.Services.LiveApi
{
    public class PlayerScores
    {
        public int Assists { get; set; }
        public int CreepScore { get; set; }
        public int Deaths { get; set; }
        public int Kills { get; set; }
        [JsonPropertyName("wardScore")]
        public decimal VisionScore { get; set; }
    }
}
