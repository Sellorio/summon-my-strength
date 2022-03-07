using SEA.Mvvm.ModelSupport;
using SummonMyStrength.Data;
using SummonMyStrength.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SummonMyStrength.Models
{
    internal class AramPreferenceModel : ModelBase<AramPreferenceViewModel>
    {
        public AramPreferenceModel(IInterface @interface, AramPreferenceViewModel viewModel)
            : base(@interface, viewModel)
        {
        }

        protected override void BindModel(ModelBinder<AramPreferenceViewModel> modelBinder)
        {
            modelBinder.Bind(x => x.AddPreferredChampion, AddPreferredChampion);
            modelBinder.Bind(x => x.RemovePreferredChampion, RemovePreferredChampion);
            modelBinder.Bind(x => x.MovePreferredChampionUp, MovePreferredChampionUp);
            modelBinder.Bind(x => x.MovePreferredChampionDown, MovePreferredChampionDown);
        }

        private void AddPreferredChampion()
        {
            if (ViewModel.SelectedOtherChampion != null)
            {
                ViewModel.PreferredChampions.Add(ViewModel.SelectedOtherChampion);
                ViewModel.SelectedPreferredChampion = ViewModel.SelectedOtherChampion;

                ViewModel.OtherChampions.Remove(ViewModel.SelectedOtherChampion);
                ViewModel.SelectedOtherChampion = null;

                Task.Run(UpdateDataStore);
            }
        }

        private void RemovePreferredChampion()
        {
            if (ViewModel.SelectedPreferredChampion != null)
            {
                ViewModel.OtherChampions.Add(ViewModel.SelectedPreferredChampion);
                ViewModel.PreferredChampions.Remove(ViewModel.SelectedPreferredChampion);
                ViewModel.SelectedPreferredChampion = null;

                Task.Run(UpdateDataStore);
            }
        }

        private void MovePreferredChampionUp()
        {
            if (ViewModel.SelectedPreferredChampion != null)
            {
                var index = ViewModel.PreferredChampions.IndexOf(ViewModel.SelectedPreferredChampion);

                if (index > 0)
                {
                    var champ = ViewModel.SelectedPreferredChampion;
                    ViewModel.PreferredChampions.Remove(champ);
                    ViewModel.PreferredChampions.Insert(index - 1, champ);
                    ViewModel.SelectedPreferredChampion = champ;

                    Task.Run(UpdateDataStore);
                }
            }
        }

        private void MovePreferredChampionDown()
        {
            if (ViewModel.SelectedPreferredChampion != null)
            {
                var index = ViewModel.PreferredChampions.IndexOf(ViewModel.SelectedPreferredChampion);

                if (index < ViewModel.PreferredChampions.Count - 1)
                {
                    var champ = ViewModel.SelectedPreferredChampion;
                    ViewModel.PreferredChampions.Remove(champ);
                    ViewModel.PreferredChampions.Insert(index + 1, champ);
                    ViewModel.SelectedPreferredChampion = champ;

                    Task.Run(UpdateDataStore);
                }
            }
        }

        private async Task UpdateDataStore()
        {
            DataStore.PreferredAramChampions = ViewModel.PreferredChampions.Select(x => int.Parse(x.Key)).ToList();
            await DataStore.SaveAsync();
        }
    }
}
