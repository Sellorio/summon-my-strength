using System.Threading.Tasks;

namespace SummonMyStrength.Api.Connectors;

public interface IDataDragonApiConnector
{
    Task<TResult> GetDataAsync<TResult>(string path, string version = "latest");
    string GetIconUrl(string path, string version = "latest");
}
