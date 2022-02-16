using SEA.Mvvm.ViewModelSupport;
using SummonMyStrength.Api.Gameflow;

namespace SummonMyStrength.ViewModels
{
    class MainViewModel : ViewModelWithModel
    {
        /// <summary>
        /// The state of the session (if relevant).
        /// </summary>
        public GameflowPhase ClientState
        {
            get => Get<GameflowPhase>();
            set => Set(value);
        }

        /// <summary>
        /// If the player is in champion select, this will store the state of the champion select process.
        /// </summary>
        public ChampionSelectSessionViewModel ChampionSelectSession
        {
            get => Get<ChampionSelectSessionViewModel>();
            set => Set(value);
        }
    }
}
