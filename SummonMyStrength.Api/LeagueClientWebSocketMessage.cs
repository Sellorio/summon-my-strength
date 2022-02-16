using System.Text.Json;

namespace SummonMyStrength.Api
{
    public class LeagueClientWebSocketMessage
    {
        public string Path { get; }
        public string Action { get; }
        public JsonElement Data { get; }

        internal LeagueClientWebSocketMessage(string path, string action, JsonElement data)
        {
            Path = path;
            Action = action;
            Data = data;
        }
    }
}
