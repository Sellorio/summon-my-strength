using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Perks
{
    public class PerksModule
    {
        private readonly LeagueClient _client;

        public event Action<PerkPage[]> PerkPagesUpdated;
        public event Action<PerkPage> PerkPageUpdated;

        public PerksModule(LeagueClient client)
        {
            _client = client;

            _client.AddMessageHandler(x =>
            {
                if (x.Path == "/lol-perks/v1/pages")
                {
                    if (x.Action == EventActions.Update && PerkPagesUpdated != null)
                    {
                        PerkPagesUpdated.Invoke(JsonSerializer.Deserialize<PerkPage[]>(x.Data.GetRawText(), _client.JsonSerializerOptions));
                    }
                }
                else if (x.Path.StartsWith("/lol-perks/v1/pages/"))
                {
                    if (x.Action == EventActions.Update && PerkPageUpdated != null)
                    {
                        PerkPageUpdated?.Invoke(JsonSerializer.Deserialize<PerkPage>(x.Data.GetRawText(), _client.JsonSerializerOptions));
                    }
                }
            });
        }

        public async Task<PerkPage> GetCurrentPageAsync()
        {
            return JsonSerializer.Deserialize<PerkPage>(await _client.HttpClient.GetStringAsync("lol-perks/v1/currentpage"), _client.JsonSerializerOptions);
        }

        public async Task SetCurrentPageAsync(int id)
        {
            var response = await _client.HttpClient.PutAsync("lol-perks/v1/currentpage", new StringContent(id.ToString()));
            response.EnsureSuccessStatusCode();
        }

        public async Task<PerkPage[]> GetPagesAsync()
        {
            return JsonSerializer.Deserialize<PerkPage[]>(await _client.HttpClient.GetStringAsync("lol-perks/v1/pages"), _client.JsonSerializerOptions);
        }

        public async Task<PerkPage> CreatePageAsync(PerkPage newPage)
        {
            var response =
                await _client.HttpClient.PostAsync(
                    "lol-perks/v1/pages",
                    new StringContent(JsonSerializer.Serialize(newPage, _client.JsonSerializerOptions), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<PerkPage>(await response.Content.ReadAsStringAsync(), _client.JsonSerializerOptions);
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
            return JsonSerializer.Deserialize<Style[]>(await _client.HttpClient.GetStringAsync("lol-perks/v1/styles"), _client.JsonSerializerOptions);
        }

        public async Task<Perk[]> GetPerksAsync()
        {
            return JsonSerializer.Deserialize<Perk[]>(await _client.HttpClient.GetStringAsync("lol-perks/v1/perks"), _client.JsonSerializerOptions);
        }
    }
}
