using System.Text.Json.Serialization;

namespace SummonMyStrength.Maui.Services.LiveApi
{
    public class GameInfo
    {
        public ActivePlayer ActivePlayer { get; set; }
        public IList<Player> AllPlayers { get; set; }
        public GameEvents Events { get; set; }
        [JsonPropertyName("gameData")]
        public GameSettings GameSettings { get; set; }

        [JsonIgnore]
        public Player CurrentPlayer => AllPlayers.First(x => x.SummonerName == ActivePlayer.SummonerName);
    }
}
