using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace SummonMyStrength.Api;

public static class HttpResponseMessageExtensions
{
    public static async Task LogIfFailedAndThrowAsync(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var responseText = await response.Content.ReadAsStringAsync();
            Trace.WriteLine("\r\nRequest has failed with status " + (int)response.StatusCode + " (" + response.StatusCode + ")\r\nContent:\r\n" + responseText + "\r\n\r\n");
            response.EnsureSuccessStatusCode();
        }
    }
}
