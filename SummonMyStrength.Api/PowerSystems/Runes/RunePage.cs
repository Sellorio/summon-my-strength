namespace SummonMyStrength.Api.PowerSystems.Runes;

public class RunePage
{
    public int Id { get; set; }
    public int[] AutoModifiedSelections { get; set; }
    public bool Current { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeletable { get; set; }
    public bool IsEditable { get; set; }
    public bool IsValid { get; set; }
    public long LastModified { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public int PrimaryStyleId { get; set; }
    public int[] SelectedPerkIds { get; set; }
    public int SubStyleId { get; set; }
}
