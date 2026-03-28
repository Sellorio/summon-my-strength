using System.Collections.Generic;

namespace SummonMyStrength.Api.PowerSystems.Items;

public class ItemDetail
{
    public string PassiveDescriptionHtml { get; set; }

    public Dictionary<ItemStatType, ItemStatValue> Stats { get; set; }
}
