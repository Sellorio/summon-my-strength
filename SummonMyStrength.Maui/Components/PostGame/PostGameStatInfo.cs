using SummonMyStrength.Api.PostGame.Stats;

namespace SummonMyStrength.Maui.Components.PostGame;

public class PostGameStatInfo(string key, string name, Func<PostGamePlayerStats, decimal> reader, string iconPath)
{
    public string Key => key;
    public string Name => name;
    public Func<PostGamePlayerStats, decimal> Reader => reader;
    public string IconPath => iconPath;
}
