namespace SummonMyStrength.Api.PowerSystems.Items;

public class ItemStatValue
{
    public decimal Value { get; set; }
    public ItemStatValueUnit Unit { get; set; }

    public override string ToString()
    {
        return Unit switch
        {
            ItemStatValueUnit.Constant => Value.ToString(),
            ItemStatValueUnit.Percent => $"{Value}%",
            ItemStatValueUnit.PerTenSeconds => $"{Value} per 10 seconds",
            _ => throw new System.NotSupportedException("Unexpected stat unit of measurement."),
        };
    }
}
