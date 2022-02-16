namespace SummonMyStrength.Api.DataDragon
{
    internal class SummonerSpellModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cooldown { get; set; }
        public ImageReferenceModel Image { get; set; }
        public string[] Modes { get; set; }
    }
}
