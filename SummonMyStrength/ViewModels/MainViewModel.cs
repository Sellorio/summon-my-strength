using SEA.Mvvm.ViewModelSupport;
using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.Gameflow;

namespace SummonMyStrength.ViewModels
{
    class MainViewModel : ViewModelWithModel
    {
        public Champion[] Champions
        {
            get => Get<Champion[]>();
            set => Set(value);
        }

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

        public int? RuneChampionContext
        {
            get => Get<int?>();
            set => Set(value);
        }

        public CommandBase Initialise
        {
            get => Get<CommandBase>();
            set => Set(value);
        }

        public CommandBase OpenApiPlayground
        {
            get => Get<CommandBase>();
            set => Set(value);
        }

        public CommandBase ReApplyChampionRunes
        {
            get => Get<CommandBase>();
            set => Set(value);
        }
    }
}
