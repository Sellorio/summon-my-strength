using SummonMyStrength.Api.Connectors.DataDragon;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Connectors;

internal class DataDragonApiConnector : IDataDragonApiConnector
{
    private static readonly JsonSerializerOptions _jsonOptions;

    static DataDragonApiConnector()
    {
        _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    private readonly HttpClient _httpClient;

    private readonly Dictionary<string, object> _cache = new();

    private DateTime? _latestVersionCachedAt;
    private string _latestVersion;

    public DataDragonApiConnector(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TResult> GetDataAsync<TResult>(string path, string version = "latest")
    {
        if (version == "latest")
        {
            await EnsureLatestVersionInfoAsync();
            version = _latestVersion;
        }

        var url = $"cdn/{version}/data/en_US/{path}";

        lock (_cache)
        {
            if (_cache.TryGetValue(url, out var cachedResult))
            {
                return (TResult)cachedResult;
            }
        }

        string json;

        try
        {
            json = await _httpClient.GetStringAsync($"cdn/{version}/data/en_US/{path}");
        }
        catch (HttpRequestException)
        {
            //TODO: logging
            throw;
        }

        var result = JsonSerializer.Deserialize<DataWrapper<TResult>>(json, _jsonOptions).Data;

        lock (_cache)
        {
            _cache[url] = result;
        }

        return result;
    }

    public string GetIconUrl(string path, string version = "latest")
    {
        if (version == "latest")
        {
            version = _latestVersion;
        }

        return $"{_httpClient.BaseAddress.AbsoluteUri}cdn/{version}/img/{path}";
    }

    private async Task EnsureLatestVersionInfoAsync()
    {
        var now = DateTime.UtcNow;

        if (_latestVersionCachedAt == null || (now - _latestVersionCachedAt.Value).Days > 0)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync("api/versions.json");
            }
            catch (HttpRequestException)
            {
                //TODO: logging
                throw;
            }

            if (!response.IsSuccessStatusCode)
            {
                //TODO: logging
                response.EnsureSuccessStatusCode();
            }
            
            _latestVersion = JsonSerializer.Deserialize<string[]>(await response.Content.ReadAsStringAsync())[0];
            _latestVersionCachedAt = now;
        }
    }
}
