using SummonMyStrength.Api.Connectors;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.PostGame;

internal class PostGameLobbyService : IPostGameLobbyService
{
    private readonly ILeagueClientApiConnector _clientApiConnector;

    public PostGameLobbyService(ILeagueClientApiConnector clientApiConnector)
    {
        _clientApiConnector = clientApiConnector;
    }

    public async Task PlayAgainAsync()
    {
        await _clientApiConnector.PostAsync("lol-lobby/v2/play-again", null);
    }
}
