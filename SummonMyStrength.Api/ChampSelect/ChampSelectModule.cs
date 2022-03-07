using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.ChampSelect
{
    public sealed class ChampSelectModule
    {
        private readonly LeagueClient _client;

        public event Func<ChampSelectSession, Task> SessionChanged;

        internal ChampSelectModule(LeagueClient client)
        {
            _client = client;

            _client.AddMessageHandler(async x =>
            {
                if ((x.Path == "/lol-lobby-team-builder/champ-select/v1/session" || x.Path == "/lol-champ-select/v1/session") && x.Action != EventActions.Delete && SessionChanged != null)
                {
                    await SessionChanged.InvokeAsync(JsonSerializer.Deserialize<ChampSelectSession>(x.Data.GetRawText(), _client.JsonSerializerOptions));
                }
            });
        }

        public async Task<ChampSelectSession> GetSessionAsync()
        {
            return
                JsonSerializer.Deserialize<ChampSelectSession>(
                    await _client.HttpClient.GetStringAsync("lol-champ-select/v1/session"),
                    _client.JsonSerializerOptions);
        }

        public async Task SwapWithBenchAsync(int championId)
        {
            var responseMessage = await _client.HttpClient.PostAsync($"lol-champ-select/v1/session/bench/swap/{championId}", new StringContent(""));
            responseMessage.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Requests a trade with a player.
        /// </summary>
        /// <param name="id">
        ///     The id of the trade contract. All possible trade partners have <see cref="ChampSelectTradeContract"/>s
        ///     created for them in the <see cref="ChampSelectSession.Trades"/> list. Simply look them up by Cell Id.
        /// </param>
        /// <returns>The updated trade contract.</returns>
        /// <remarks>
        /// It appears that all trade ids are a number incrementing from 0 unique across all players in champ select. Trade ids
        /// will thus be in the ranges of 0-3, 4-7, etc (since there are 4 players besides yourself).
        /// </remarks>
        public async Task<ChampSelectTradeContract> RequestTradeAsync(long id)
        {
            var responseMessage = await _client.HttpClient.PostAsync($"lol-champ-select/v1/session/trades/{id}/request", new StringContent(""));
            responseMessage.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<ChampSelectTradeContract>(await responseMessage.Content.ReadAsStringAsync(), _client.JsonSerializerOptions);
        }

        /// <summary>
        /// Retrieves the skins that should appear in the skin carousel (i.e. the skins for the locked in champion).
        /// Will return empty if there is no locked in champion.
        /// </summary>
        /// <returns>The retrieved skins.</returns>
        public async Task<IList<SkinSelectorSkin>> GetSkinCarouselSkinsAsync()
        {
            return
                JsonSerializer.Deserialize<IList<SkinSelectorSkin>>(
                    await _client.HttpClient.GetStringAsync("lol-champ-select/v1/skin-carousel-skins"),
                    _client.JsonSerializerOptions);
        }

        /// <summary>
        /// Updates one or more fields of the <see cref="MySelection"/> data. Use this to update
        /// summoner spells, skins or ward skins.
        /// </summary>
        /// <param name="mySelection">The partial or full set of new <see cref="MySelection"/> data to use.</param>
        /// <returns>The task.</returns>
        public async Task PatchMySelectionAsync(MySelection mySelection)
        {
            var responseMessage =
                await _client.HttpClient.PatchAsync(
                    "lol-champ-select/v1/session/my-selection",
                    new StringContent(JsonSerializer.Serialize(mySelection, _client.JsonSerializerOptions), Encoding.UTF8, "application/json"));

            responseMessage.EnsureSuccessStatusCode();
        }
    }
}
