using SummonMyStrength.Api.DataDragon;

namespace SummonMyStrength.Api.Champions
{
    public class Champion
    {
        public string Version { get; set; }
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Blurb { get; set; }
        public ChampionInfo Info { get; set; }
        public ImageReference Image { get; set; }
        public string[] Tags { get; set; }
        public string Partype { get; set; }
        public ChampionStats Stats { get; set; }
    }
}
