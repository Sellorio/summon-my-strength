using System.Collections.Generic;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Social;

public interface IFriendService
{
    Task<IList<Friend>> GetFriendsAsync();
}
