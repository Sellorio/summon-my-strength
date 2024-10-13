using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Perks;

public class PerksModule
{
    private readonly LeagueClient _client;

    public event Func<PerkPage[], Task> PerkPagesUpdated;
    public event Func<PerkPage, Task> PerkPageUpdated;

    public PerksModule(LeagueClient client)
    {
        _client = client;

        _client.AddMessageHandler(async x =>
        {
            if (x.Path == "/lol-perks/v1/pages")
            {
                if (x.Action == EventActions.Update && PerkPagesUpdated != null)
                {
                    await PerkPagesUpdated.InvokeAsync(JsonSerializer.Deserialize<PerkPage[]>(x.Data.GetRawText(), LeagueClient.JsonSerializerOptions));
                }
            }
            else if (x.Path.StartsWith("/lol-perks/v1/pages/"))
            {
                if (x.Action == EventActions.Update && PerkPageUpdated != null)
                {
                    await PerkPageUpdated.InvokeAsync(JsonSerializer.Deserialize<PerkPage>(x.Data.GetRawText(), LeagueClient.JsonSerializerOptions));
                }
            }
        });
    }

    public async Task<PerkPage> GetCurrentPageAsync()
    {
        return JsonSerializer.Deserialize<PerkPage>(await _client.HttpClient.GetStringAsync("lol-perks/v1/currentpage"), LeagueClient.JsonSerializerOptions);
    }

    public async Task SetCurrentPageAsync(int id)
    {
        var response = await _client.HttpClient.PutAsync("lol-perks/v1/currentpage", new StringContent(id.ToString()));
        response.EnsureSuccessStatusCode();
    }

    public async Task<PerkPage[]> GetPagesAsync()
    {
        return JsonSerializer.Deserialize<PerkPage[]>(await _client.HttpClient.GetStringAsync("lol-perks/v1/pages"), LeagueClient.JsonSerializerOptions);
    }

    public async Task<PerkPage> CreatePageAsync(PerkPage newPage)
    {
        var response =
            await _client.HttpClient.PostAsync(
                "lol-perks/v1/pages",
                new StringContent(JsonSerializer.Serialize(newPage, LeagueClient.JsonSerializerOptions), Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<PerkPage>(await response.Content.ReadAsStringAsync(), LeagueClient.JsonSerializerOptions);
    }

    public async Task DeletePageAsync(int id)
    {
        var response = await _client.HttpClient.DeleteAsync($"lol-perks/v1/pages/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAllPagesAsync()
    {
        var response = await _client.HttpClient.DeleteAsync("lol-perks/v1/pages");
        response.EnsureSuccessStatusCode();
    }

    public async Task<Style[]> GetStylesAsync()
    {
        return JsonSerializer.Deserialize<Style[]>(await _client.HttpClient.GetStringAsync("lol-perks/v1/styles"), LeagueClient.JsonSerializerOptions);
    }

    public async Task<Perk[]> GetPerksAsync()
    {
        return JsonSerializer.Deserialize<Perk[]>(await _client.HttpClient.GetStringAsync("lol-perks/v1/perks"), LeagueClient.JsonSerializerOptions);
    }
}
