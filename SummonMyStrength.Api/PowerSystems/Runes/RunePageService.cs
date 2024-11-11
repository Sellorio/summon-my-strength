using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.Connectors.WebSocket;
using System;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PowerSystems.Runes;

internal class RunePageService : IRunePageService, IDisposable
{
    private readonly ILeagueClientApiConnector _clientApiConnector;
    private readonly ILeagueClientWebSocketConnector _clientWebSocketConnector;

    public event Func<RunePage, Task> RunePageUpdated;
    public event Func<RunePage[], Task> RunePagesUpdated;

    public RunePageService(ILeagueClientApiConnector clientApiConnector, ILeagueClientWebSocketConnector clientWebSocketConnector)
    {
        _clientApiConnector = clientApiConnector;
        _clientWebSocketConnector = clientWebSocketConnector;

        _clientWebSocketConnector.AddMessageHandler<RunePage>(
            this,
            MessageId.RunePage,
            MessageAction.Update,
            x => RunePageUpdated.InvokeAsync(x));

        _clientWebSocketConnector.AddMessageHandler<RunePage[]>(
            this,
            MessageId.RunePages,
            MessageAction.Update,
            x => RunePagesUpdated.InvokeAsync(x));
    }

    public void Dispose()
    {
        _clientWebSocketConnector.RemoveAllMessageHandlers(this);
    }

    public async Task<RunePage> GetCurrentPageAsync()
    {
        return await _clientApiConnector.GetAsync<RunePage>("lol-perks/v1/currentpage");
    }

    public async Task SetCurrentPageAsync(int id)
    {
        await _clientApiConnector.PutStringAsync("lol-perks/v1/currentpage", id.ToString());
    }

    public async Task<RunePage[]> GetPagesAsync()
    {
        return await _clientApiConnector.GetAsync<RunePage[]>("lol-perks/v1/pages");
    }

    public async Task<RunePage> CreatePageAsync(RunePage newPage)
    {
        return await _clientApiConnector.PostAsync<RunePage>("lol-perks/v1/pages", newPage);
    }

    public async Task DeletePageAsync(int id)
    {
        await _clientApiConnector.DeleteAsync($"lol-perks/v1/pages/{id}");
    }

    public async Task DeleteAllPagesAsync()
    {
        await _clientApiConnector.DeleteAsync("lol-perks/v1/pages");
    }
}
