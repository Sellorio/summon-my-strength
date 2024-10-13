using System.Threading.Tasks;

namespace SummonMyStrength.Api.Connectors;

public interface ILeagueClientApiConnector
{
    Task<TResult> GetAsync<TResult>(string url);
    Task PostAsync(string url, object body);
}
