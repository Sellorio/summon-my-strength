namespace SummonMyStrength.Api.Connectors.DataDragon;

public class DataWrapper<TData>
{
    public string Type { get; set; }
    public string Format { get; set; }
    public string Version { get; set; }
    public TData Data { get; set; }
}
