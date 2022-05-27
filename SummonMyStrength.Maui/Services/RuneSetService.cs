using SummonMyStrength.Api;
using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.Gameflow;
using SummonMyStrength.Api.Perks;

namespace SummonMyStrength.Maui.Services
{
    internal class RuneSetService : IRuneSetService
    {
        private readonly LeagueClient _leagueClient;

        private Champion _runesLoadedForChampion;
        private DateTime _runesLoadedAt;

        public bool HasUnsavedChanges { get; private set; }

        public event Func<Task> HasUnsavedChangesChanged;

        public RuneSetService(LeagueClient leagueClient)
        {
            _leagueClient = leagueClient;

            _leagueClient.Disconnected += async () =>
            {
                _runesLoadedForChampion = null;

                if (HasUnsavedChanges)
                {
                    HasUnsavedChanges = false;
                    await HasUnsavedChangesChanged.InvokeAsync();
                }
            };

            _leagueClient.Gameflow.GameflowPhaseChanged += async newPhase =>
            {
                if (newPhase == GameflowPhase.ChampSelect || newPhase == GameflowPhase.EndOfGame)
                {
                    await LoadRunesForChampionAsync(null);

                    if (HasUnsavedChanges)
                    {
                        HasUnsavedChanges = false;
                        await HasUnsavedChangesChanged.InvokeAsync();
                    }
                }
            };

            _leagueClient.Perks.PerkPageUpdated += async _ =>
            {
                if (_runesLoadedForChampion != null && HasUnsavedChanges == false && DateTime.Now - _runesLoadedAt > TimeSpan.FromSeconds(2))
                {
                    HasUnsavedChanges = true;
                    await HasUnsavedChangesChanged.InvokeAsync();
                }
            };

            _leagueClient.Perks.PerkPagesUpdated += async _ =>
            {
                if (_runesLoadedForChampion != null && HasUnsavedChanges == false && DateTime.Now - _runesLoadedAt > TimeSpan.FromSeconds(2))
                {
                    HasUnsavedChanges = true;
                    await HasUnsavedChangesChanged.InvokeAsync();
                }
            };
        }

        public async Task LoadRunesForChampionAsync(Champion champion)
        {
            if (champion == _runesLoadedForChampion)
            {
                return;
            }

            var runePages = Array.Empty<PerkPage>();

            if (champion != null)
            {
                var key = int.Parse(champion.Key);

                if (DataStore.RunePages.ContainsKey(key))
                {
                    runePages = DataStore.RunePages[key];
                }
            }

            await _leagueClient.Perks.DeleteAllPagesAsync();

            foreach (var page in runePages)
            {
                page.Id = (await _leagueClient.Perks.CreatePageAsync(page)).Id;
            }

            _runesLoadedForChampion = champion;
            _runesLoadedAt = DateTime.Now;
        }

        public async Task SaveRunes()
        {
            if (_runesLoadedForChampion == null)
            {
                return;
            }

            if (!HasUnsavedChanges)
            {
                return;
            }

            var pages = await _leagueClient.Perks.GetPagesAsync();
            DataStore.RunePages[int.Parse(_runesLoadedForChampion.Key)] = pages.Where(x => x.IsDeletable).ToArray();
            await DataStore.SaveAsync();

            HasUnsavedChanges = false;
            await HasUnsavedChangesChanged.InvokeAsync();
        }
    }
}
