using SummonMyStrength.Api.ChampSelect;

namespace SummonMyStrength.Maui.Services
{
    public interface IChampSelectSessionAccessor
    {
        public ChampSelectSession Session { get; }
        public event Func<ChampSelectSession, ChampSelectSession, Task> SessionChanged;
    }
}
