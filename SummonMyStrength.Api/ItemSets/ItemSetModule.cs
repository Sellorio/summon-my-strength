using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.ItemSets
{
    public class ItemSetModule
    {
        private readonly LeagueClient _client;

        public event Func<long, ItemSetList, Task> ItemSetsChanged;

        internal ItemSetModule(LeagueClient client)
        {
            _client = client;

            _client.AddMessageHandler(async x =>
            {
                // /lol-item-sets/v1/item-sets/332779/sets
                if (x.Path.StartsWith("/lol-item-sets/v1/item-sets"))
                {
                    var summonerId = long.Parse(Regex.Match(x.Path, @"\/lol-item-sets\/v1\/item-sets\/([0-9]+)\/sets").Groups[1].Value);
                    await ItemSetsChanged.InvokeAsync(summonerId, JsonSerializer.Deserialize<ItemSetList>(x.Data.GetRawText(), LeagueClient.JsonSerializerOptions));
                }
            });
        }

        public async Task UpdateItemSetsAsync(long summonerId, ItemSetList itemSets)
        {
            var response =
                await _client.HttpClient.PutAsync(
                    $"lol-item-sets/v1/item-sets/{summonerId}/sets",
                    new StringContent(JsonSerializer.Serialize(itemSets, LeagueClient.JsonSerializerOptions), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }
    }
}
