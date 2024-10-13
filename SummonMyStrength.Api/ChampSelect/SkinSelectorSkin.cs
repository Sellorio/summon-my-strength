namespace SummonMyStrength.Api.ChampSelect;

public class SkinSelectorSkin
{
    public int ChampionId { get; set; }
    public SkinSelectorChildSkin[] ChildSkins { get; set; }
    public string ChromaPreviewPath { get; set; }
    public bool Disabled { get; set; }
    public CollectionsSkinEmblem[] Emblems { get; set; }
    public string GroupSplash { get; set; }
    public int Id { get; set; }
    public bool IsBase { get; set; }
    public bool IsChampionUnlocked { get; set; }
    public bool IsUnlockedFromEntitledFeature { get; set; }
    public string Name { get; set; }
    public CollectionsOwnership Ownership { get; set; }
    public string RarityGemPath { get; set; }
    public string SplashPath { get; set; }
    public string SplashVideoPath { get; set; }
    public bool StillObtainable { get; set; }
    public string TilePath { get; set; }
    public bool Unlocked { get; set; }
}
