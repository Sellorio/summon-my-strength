namespace SummonMyStrength.Api.ItemSets
{
    public class ItemSetBlock
    {
        public string HideIfSummonerSpell { get; set; }
        public ItemSetItem[] Items { get; set; }
        public string ShowIfSummonerSpell { get; set; }
        public string Type { get; set; }
    }
}
