namespace SummonMyStrength.Maui.Data;

public class HandsFreePreference
{
    public int PickChampionId { get; set; }
    public List<int> BanChampionIds { get; set; }
    public int SummonerSpell1 { get; set; }
    public int SummonerSpell2 { get; set; }
}