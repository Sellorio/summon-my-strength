namespace SummonMyStrength.Maui.Services.LiveApi;

public class PlayerRunes
{
    public IList<PlayerRune> GeneralRunes { get; set; }
    public PlayerRune Keystone { get; set; }
    public PlayerRune PrimaryRuneTree { get; set; }
    public PlayerRune SecondaryRuneTree { get; set; }
    public IList<PlayerRune> StatRunes { get; set; }
}
