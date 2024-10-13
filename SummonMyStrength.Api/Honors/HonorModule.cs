using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SummonMyStrength.Api.PostGame.Honors;

namespace SummonMyStrength.Api.Honors;

public class HonorModule
{
    private readonly LeagueClient _client;

    public event Func<HonorBallot, Task> HonorBallotCreated;

    internal HonorModule(LeagueClient client)
    {
        _client = client;
        _client.AddMessageHandler(async message =>
        {
            if (message.Path == "/lol-honor-v2/v1/ballot" && message.Action == "Create" && HonorBallotCreated != null)
            {
                await HonorBallotCreated.InvokeAsync(
                    JsonSerializer.Deserialize<HonorBallot>(message.Data, LeagueClient.JsonSerializerOptions));
            }
        });
    }

    public async Task<HonorBallot> GetBallotAsync()
    {
        return JsonSerializer.Deserialize<HonorBallot>(await _client.HttpClient.GetStringAsync("lol-honor-v2/v1/ballot"), LeagueClient.JsonSerializerOptions);
    }

    public async Task HonorPlayerAsync(PlayerHonor honor)
    {
        var response1 =
            await _client.HttpClient.PostAsync(
                "lol-honor-v2/v1/honor-player",
                new StringContent(JsonSerializer.Serialize(honor, LeagueClient.JsonSerializerOptions), Encoding.UTF8, "application/json"));

        var response2 =
            await _client.HttpClient.PostAsync(
                "lol-honor/v1/honor",
                new StringContent(JsonSerializer.Serialize(new { recipientPuuid = honor.Puuid, honorType = honor.HonorType }, LeagueClient.JsonSerializerOptions), Encoding.UTF8, "application/json"));

        //response.EnsureSuccessStatusCode();
    }
}
