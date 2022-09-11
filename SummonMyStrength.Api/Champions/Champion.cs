using SummonMyStrength.Api.DataDragon;
using System.Text.Json.Serialization;

namespace SummonMyStrength.Api.Champions
{
    public class Champion
    {
        public string Version { get; set; }

        [JsonPropertyName("id")]
        public string Code { get; set; }

        [JsonPropertyName("key")]
        public string IdString
        {
            get
            {
                return Id.ToString();
            }
            set
            {
                Id = int.Parse(value);
            }
        }

        public string Name { get; set; }
        public string Title { get; set; }
        public string Blurb { get; set; }
        public ChampionInfo Info { get; set; }
        public ImageReference Image { get; set; }
        public string[] Tags { get; set; }
        public string Partype { get; set; }
        public ChampionStats Stats { get; set; }

        [JsonIgnore]
        public int Id { get; set; }
    }
}
