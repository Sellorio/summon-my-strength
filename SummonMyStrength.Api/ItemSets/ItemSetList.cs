namespace SummonMyStrength.Api.ItemSets
{
    public class ItemSetList
    {
        public long AccountId { get; set; }
        public ItemSet[] ItemSets { get; set; }
        public long Timestamp { get; set; }
    }
}
