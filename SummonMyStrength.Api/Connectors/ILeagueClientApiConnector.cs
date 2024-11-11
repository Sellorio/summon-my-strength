using System.Threading.Tasks;

namespace SummonMyStrength.Api.Connectors;

public interface ILeagueClientApiConnector
{
    Task<TResult> GetAsync<TResult>(string url);
    Task PostAsync(string url, object body);
    Task<TResult> PostAsync<TResult>(string url, object body);
    Task PutAsync(string url, object body);
    Task PutStringAsync(string url, string content);
    Task PatchAsync(string url, object body);
    Task DeleteAsync(string url);
}
