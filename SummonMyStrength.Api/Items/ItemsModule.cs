using SummonMyStrength.Api.Champions;
using SummonMyStrength.Api.DataDragon;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Items;

public class ItemsModule
{
    private readonly LeagueClient _client;
    private string _dataDragonVersion;
    private Dictionary<string, Item> _itemCache;

    public ItemsModule(LeagueClient client)
    {
        _client = client;
    }

    public async Task<Dictionary<string, Item>> GetItemsAsync()
    {
        if (_itemCache != null)
        {
            return _itemCache;
        }

        var version = _dataDragonVersion ??= JsonSerializer.Deserialize<string[]>(await _client.DataDragonHttpClient.GetStringAsync("api/versions.json"))[0];
        var json = await _client.DataDragonHttpClient.GetStringAsync($"cdn/{version}/data/en_US/item.json");
        var itemData = JsonSerializer.Deserialize<DataWrapper<Dictionary<string, Item>>>(json, LeagueClient.JsonSerializerOptions);

        return _itemCache = itemData.Data;
    }

    public string GetIconUrl(Champion champion)
    {
        return $"{_client.DataDragonHttpClient.BaseAddress.AbsoluteUri}cdn/{_dataDragonVersion}/img/item/{champion.Image.Full}";
    }
}
