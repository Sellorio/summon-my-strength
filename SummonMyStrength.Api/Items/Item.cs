using System.Collections.Generic;

namespace SummonMyStrength.Api.Items;

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Colloq { get; set; }
    public string Plaintext { get; set; }
    public string[] From { get; set; }
    public ItemImage Image { get; set; }
    public ItemGold Gold { get; set; }
    public string[] Tags { get; set; }
    public Dictionary<string, bool> Maps { get; set; }
    public Dictionary<string, decimal> Stats { get; set; }
    public int Depth { get; set; }
}
