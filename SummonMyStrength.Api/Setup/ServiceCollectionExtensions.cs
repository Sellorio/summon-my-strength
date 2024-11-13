using Microsoft.Extensions.DependencyInjection;
using SummonMyStrength.Api.ChampSelect;
using SummonMyStrength.Api.Collections.Champions;
using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.General;
using SummonMyStrength.Api.Matchmaking;
using SummonMyStrength.Api.PostGame;
using SummonMyStrength.Api.PostGame.Honors;
using SummonMyStrength.Api.PostGame.Stats;
using SummonMyStrength.Api.PowerSystems.Runes;
using SummonMyStrength.Api.PowerSystems.SummonerSpells;
using SummonMyStrength.Api.Social;
using System.Net.Http;
using System.Security.Authentication;

namespace SummonMyStrength.Api.Setup;

public static class ServiceCollectionExtensions
{
    private const string DataDragonHttpClientName = "DataDragon";
    private const string ClientApiHttpClientName = "ApiClient";

    public static IServiceCollection AddLeagueOfLegendsServices(this IServiceCollection services)
    {
        var noSslValidationMessageHandler = new HttpClientHandler
        {
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
            ServerCertificateCustomValidationCallback = (a, b, c, d) => true
        };

        services
            .AddHttpClient(DataDragonHttpClientName)
            .ConfigurePrimaryHttpMessageHandler(x => noSslValidationMessageHandler)
            .ConfigureHttpClient(x => x.BaseAddress = new("http://ddragon.leagueoflegends.com/"));

        services
            .AddHttpClient(ClientApiHttpClientName)
            .ConfigurePrimaryHttpMessageHandler(x => noSslValidationMessageHandler);

        services
            .AddSingleton<ILeagueConnectionSettingsProvider, LeagueConnectionSettingsProvider>()
            .AddSingleton<IDataDragonApiConnector>(svc =>
                new DataDragonApiConnector(
                    svc.GetRequiredService<IHttpClientFactory>().CreateClient(DataDragonHttpClientName)))
            .AddSingleton<ILeagueClientApiConnector>(svc =>
                new LeagueClientApiConnector(
                    svc.GetRequiredService<IHttpClientFactory>().CreateClient(ClientApiHttpClientName),
                    svc.GetRequiredService<ILeagueConnectionSettingsProvider>()))
            .AddSingleton<ILeagueClientWebSocketConnector, LeagueClientWebSocketConnector>()
            .AddSingleton<ILeagueLiveApiConnector, LeagueLiveApiConnector>()
            ;

        services
            // ChampSelect
            .AddScoped<IChampSelectSessionService, ChampSelectSessionService>()
            .AddScoped<IChampSelectBenchService, ChampSelectBenchService>()
            .AddScoped<IChampSelectChampionTradeService, ChampSelectChampionTradeService>()
            .AddScoped<IChampSelectPickOrderSwapService, ChampSelectPickOrderSwapService>()
            .AddScoped<IChampSelectActionService, ChampSelectActionService>()

            // Collections
            .AddScoped<IChampionService, ChampionService>()

            // General
            .AddScoped<IGameflowService, GameflowService>()

            // Matchmaking
            .AddScoped<IReadyCheckService, ReadyCheckService>()

            // Post Game
            .AddScoped<IPostGameLobbyService, PostGameLobbyService>()
            .AddScoped<IHonorService, HonorService>()
            .AddScoped<IPostGameStatsService, PostGameStatsService>()

            // Power Systems
            .AddScoped<IRunePageService, RunePageService>()
            .AddScoped<ISummonerSpellService, SummonerSpellService>()
            .AddScoped<ISummonerSpellSelectionService, SummonerSpellSelectionService>()

            // Social
            .AddScoped<IFriendService, FriendService>()
            .AddScoped<IPlayerReportService, PlayerReportService>()
            ;

        return services;
    }
}
