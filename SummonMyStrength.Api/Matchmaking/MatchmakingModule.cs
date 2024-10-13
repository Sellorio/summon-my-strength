using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Matchmaking;

public class MatchmakingModule
{
    private readonly LeagueClient _client;

    public event Func<ReadyCheck, Task> ReadyCheckChanged;
    public event Func<Task> ReadyCheckEnded;

    internal MatchmakingModule(LeagueClient client)
    {
        _client = client;

        _client.AddMessageHandler(async x =>
        {
            if (x.Path == "/lol-matchmaking/v1/ready-check")
            {
                if (x.Action == EventActions.Update)
                {
                    await ReadyCheckChanged.InvokeAsync(
                        JsonSerializer.Deserialize<ReadyCheck>(x.Data.GetRawText(), LeagueClient.JsonSerializerOptions));
                }
                else if (x.Action == EventActions.Delete)
                {
                    await ReadyCheckEnded.InvokeAsync();
                }
            }
        });
    }

    public async Task<ReadyCheck> GetReadyCheckAsync()
    {
        return JsonSerializer.Deserialize<ReadyCheck>(await _client.HttpClient.GetStringAsync("lol-matchmaking/v1/ready-check"), LeagueClient.JsonSerializerOptions);
    }

    public async Task AcceptReadyCheckAsync()
    {
        var response = await _client.HttpClient.PostAsync("lol-matchmaking/v1/ready-check/accept", new StringContent(string.Empty));
        response.EnsureSuccessStatusCode();
    }
}
