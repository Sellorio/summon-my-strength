using SEA.Mvvm.ViewModelSupport;
using SummonMyStrength.Api.Champions;
using System.Collections.ObjectModel;

namespace SummonMyStrength.ViewModels
{
    public class AramPreferenceViewModel : ViewModelWithModel
    {
        public ObservableCollection<Champion> PreferredChampions
        {
            get => Get<ObservableCollection<Champion>>();
            set => Set(value);
        }

        public Champion SelectedPreferredChampion
        {
            get => Get<Champion>();
            set => Set(value);
        }

        public ObservableCollection<Champion> OtherChampions
        {
            get => Get<ObservableCollection<Champion>>();
            set => Set(value);
        }

        public Champion SelectedOtherChampion
        {
            get => Get<Champion>();
            set => Set(value);
        }

        public CommandBase AddPreferredChampion
        {
            get => Get<CommandBase>();
            set => Set(value);
        }

        public CommandBase RemovePreferredChampion
        {
            get => Get<CommandBase>();
            set => Set(value);
        }

        public CommandBase MovePreferredChampionUp
        {
            get => Get<CommandBase>();
            set => Set(value);
        }

        public CommandBase MovePreferredChampionDown
        {
            get => Get<CommandBase>();
            set => Set(value);
        }
    }
}
