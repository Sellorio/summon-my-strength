namespace SummonMyStrength.Api.ItemSets;

public class ItemSet
{
    public int[] AssociatedChampions { get; set; }
    public int[] AssociatedMaps { get; set; }
    public ItemSetBlock[] Blocks { get; set; }
    public string Map { get; set; }
    public string Mode { get; set; }
    public ItemSetPreferredItemSlot[] PreferredItemSlots { get; set; }
    public int SortRank { get; set; }
    public string StartedFrom { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string Uid { get; set; }
}
