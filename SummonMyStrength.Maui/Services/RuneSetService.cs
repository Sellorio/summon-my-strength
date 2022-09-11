using SummonMyStrength.Api;
using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.Gameflow;
using SummonMyStrength.Api.Perks;

namespace SummonMyStrength.Maui.Services
{
    internal class RuneSetService : IRuneSetService
    {
        private readonly LeagueClient _leagueClient;

        private bool _switchingRunes;
        private Champion _runesLoadingFor;
        private DateTime _runesLoadedAt;

        public Champion RunesLoadedFor { get; private set; }

        public event Func<Task> RunesLoadedForChanged;

        public RuneSetService(LeagueClient leagueClient)
        {
            _leagueClient = leagueClient;

            _leagueClient.Disconnected += async () =>
            {
                RunesLoadedFor = null;
                await RunesLoadedForChanged.InvokeAsync();
            };

            _leagueClient.Gameflow.GameflowPhaseChanged += async newPhase =>
            {
                if (newPhase == GameflowPhase.ChampSelect || newPhase == GameflowPhase.EndOfGame)
                {
                    await LoadRunesForChampionAsync(null);
                }
            };

            _leagueClient.Perks.PerkPageUpdated += async _ =>
            {
                if (RunesLoadedFor != null && !_switchingRunes && DateTime.Now - _runesLoadedAt > TimeSpan.FromSeconds(2))
                {
                    await SaveRunes();
                }
            };

            _leagueClient.Perks.PerkPagesUpdated += async _ =>
            {
                if (RunesLoadedFor != null && !_switchingRunes && DateTime.Now - _runesLoadedAt > TimeSpan.FromSeconds(2))
                {
                    await SaveRunes();
                }
            };
        }

        public async Task LoadRunesForChampionAsync(Champion champion)
        {
            if (champion == RunesLoadedFor || champion == _runesLoadingFor)
            {
                return;
            }

            if (!_leagueClient.IsConnected)
            {
                return;
            }

            var runePages = Array.Empty<PerkPage>();

            if (champion != null)
            {
                if (DataStore.RunePages.ContainsKey(champion.Id))
                {
                    runePages = DataStore.RunePages[champion.Id];
                }
            }

            _switchingRunes = true;
            _runesLoadingFor = champion;

            await _leagueClient.Perks.DeleteAllPagesAsync();

            foreach (var page in runePages)
            {
                // runes being changed in another thread to a new champ
                if (_runesLoadingFor != champion)
                {
                    return;
                }

                page.Id = (await _leagueClient.Perks.CreatePageAsync(page)).Id;
            }

            RunesLoadedFor = champion;
            _runesLoadedAt = DateTime.Now;
            _switchingRunes = false;
            _runesLoadingFor = null;

            await RunesLoadedForChanged.InvokeAsync();
        }

        private async Task SaveRunes()
        {
            var pages = await _leagueClient.Perks.GetPagesAsync();
            DataStore.RunePages[RunesLoadedFor.Id] = pages.Where(x => x.IsDeletable).ToArray();
            await DataStore.SaveAsync();
        }
    }
}
