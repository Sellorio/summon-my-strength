using SEA.Mvvm.ViewModelSupport;
using SummonMyStrength.Api.ChampSelect;

namespace SummonMyStrength.ViewModels
{
    class ChampionSelectSessionViewModel : ViewModel
    {
        public ChampSelectAssignedPosition? Position
        {
            get => Get<ChampSelectAssignedPosition?>();
            set => Set(value);
        }

        public int SelectedChampionId
        {
            get => Get<int>();
            set => Set(value);
        }
    }
}
