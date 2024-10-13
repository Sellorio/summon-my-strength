using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PostGame;

public class PostGameModule
{
    private readonly LeagueClient _client;

    public event Func<PostGameStats, Task> PostGameStatsPosted;
    public event Func<string, Task> PreEndOfGamePhaseChanged;

    internal PostGameModule(LeagueClient client)
    {
        _client = client;
        _client.AddMessageHandler(async message =>
        {
            if (message.Path == "/lol-end-of-game/v1/eog-stats-block" && message.Action != EventActions.Delete && PostGameStatsPosted != null)
            {
                await PostGameStatsPosted.InvokeAsync(
                    JsonSerializer.Deserialize<PostGameStats>(message.Data, LeagueClient.JsonSerializerOptions));
            }
            else if (message.Path == "/lol-pre-end-of-game/v1/currentSequenceEvent" && message.Action == EventActions.Update && PreEndOfGamePhaseChanged != null)
            {
                var data = JsonSerializer.Deserialize<PreEndOfGameSequenceMessageBody>(message.Data, LeagueClient.JsonSerializerOptions);
                await PreEndOfGamePhaseChanged.InvokeAsync(data.Name);
            }
        });
    }

    public async Task DismissStatsAsync()
    {
        var response = await _client.HttpClient.PostAsync("lol-end-of-game/v1/state/dismiss-stats", new StringContent(""));
        response.EnsureSuccessStatusCode();
    }
}
