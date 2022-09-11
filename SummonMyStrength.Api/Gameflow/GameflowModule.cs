using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Gameflow
{
    public sealed class GameflowModule
    {
        private readonly LeagueClient _client;

        internal Func<GameflowPhase, Task> GameflowPhaseChangedDelegate;

        public event Func<GameflowPhase, Task> GameflowPhaseChanged;

        internal GameflowModule(LeagueClient client)
        {
            _client = client;

            _client.AddMessageHandler(async x =>
            {
                if (x.Path == "/lol-gameflow/v1/gameflow-phase")
                {
                    var phaseAsString = x.Data.GetString();

                    if (!Enum.TryParse<GameflowPhase>(phaseAsString, out var phase))
                    {
                        phase = GameflowPhase.NewUnsupportedValue;
                    }

                    await GameflowPhaseChanged.InvokeAsync(phase);
                }
            });
        }

        public async Task<GameflowPhase> GetGameflowPhaseAsync()
        {
            var phaseAsString = await _client.HttpClient.GetStringAsync("lol-gameflow/v1/gameflow-phase");

            if (!Enum.TryParse<GameflowPhase>(phaseAsString.Trim('"'), out var phase))
            {
                phase = GameflowPhase.NewUnsupportedValue;
            }

            return phase;
        }
    }
}
