using SummonMyStrength.Api.Connectors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Social;

public class FriendService : IFriendService
{
    private readonly ILeagueClientApiConnector _clientApiConnector;

    public FriendService(ILeagueClientApiConnector clientApiConnector)
    {
        _clientApiConnector = clientApiConnector;
    }

    public async Task<IList<Friend>> GetFriendsAsync()
    {
        return await _clientApiConnector.GetAsync<IList<Friend>>("lol-chat/v1/friends");
    }
}
