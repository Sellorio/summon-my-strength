using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Connectors;

internal class LeagueClientApiConnector : ILeagueClientApiConnector
{
    private static readonly JsonSerializerOptions _jsonOptions;

    static LeagueClientApiConnector()
    {
        _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    private readonly HttpClient _httpClient;
    private readonly ILeagueConnectionSettingsProvider _connectionSettingsProvider;

    public LeagueClientApiConnector(HttpClient httpClient, ILeagueConnectionSettingsProvider connectionSettingsProvider)
    {
        _connectionSettingsProvider = connectionSettingsProvider;
        _httpClient = httpClient;
    }

    public async Task<TResult> GetAsync<TResult>(string url)
    {
        var response = await SendRequestAsync(HttpMethod.Get, url);

        if (response == null || !response.IsSuccessStatusCode)
        {
            return default;
        }

        return JsonSerializer.Deserialize<TResult>(await response.Content.ReadAsStringAsync(), _jsonOptions);
    }

    public async Task PostAsync(string url, object body)
    {
        await SendRequestAsync(HttpMethod.Post, url, body);
    }

    public async Task<TResult> PostAsync<TResult>(string url, object body)
    {
        var response = await SendRequestAsync(HttpMethod.Post, url, body);

        if (response == null || !response.IsSuccessStatusCode)
        {
            return default;
        }

        return JsonSerializer.Deserialize<TResult>(await response.Content.ReadAsStringAsync(), _jsonOptions);
    }

    public async Task PutAsync(string url, object body)
    {
        await SendRequestAsync(HttpMethod.Put, url, body);
    }

    public async Task PutStringAsync(string url, string content)
    {
        await SendRequestAsync(HttpMethod.Put, url, content, bodyAsRawText: true);
    }

    public async Task PatchAsync(string url, object body)
    {
        await SendRequestAsync(HttpMethod.Patch, url, body);
    }

    public async Task DeleteAsync(string url)
    {
        await SendRequestAsync(HttpMethod.Delete, url);
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string url, object body = null, bool bodyAsRawText = false)
    {
        LeagueConnectionSettings settings = null;

        if (!await _connectionSettingsProvider.TryReadAsync(x => settings = x))
        {
            return null;
        }

        var baseAddress = $"https://127.0.0.1:{settings.PortNumber}";
        var authenticationToken = Convert.ToBase64String(Encoding.ASCII.GetBytes("riot:" + settings.Password));

        var request = new HttpRequestMessage(method, new Uri($"{baseAddress}/{url}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authenticationToken);

        if (method == HttpMethod.Get || method == HttpMethod.Delete)
        {
        }
        else if (body == null)
        {
            request.Content = new StringContent("");
        }
        else if (bodyAsRawText)
        {
            request.Content = new StringContent((string)body, Encoding.UTF8);
        }
        else
        {
            request.Content =
                new StringContent(
                    JsonSerializer.Serialize(body, _jsonOptions),
                    Encoding.UTF8,
                    "application/json");
        }

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.SendAsync(request);
        }
        catch (HttpRequestException)
        {
            //TODO: logging
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            //TODO: logging
        }

        return response;
    }
}
