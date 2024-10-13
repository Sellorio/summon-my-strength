using Microsoft.Extensions.DependencyInjection;
using SummonMyStrength.Api.Connectors;
using SummonMyStrength.Api.General;
using SummonMyStrength.Api.PostGame.Honors;
using System.Net.Http;
using System.Security.Authentication;

namespace SummonMyStrength.Api.Setup;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddLeagueOfLegendsServices(this IServiceCollection services)
    {
        var dataDragonHttpMessageHandler = new HttpClientHandler
        {
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
            ServerCertificateCustomValidationCallback = (a, b, c, d) => true
        };

        services
            .AddHttpClient(Config.DataDragonHttpClientName)
            .ConfigurePrimaryHttpMessageHandler(x => dataDragonHttpMessageHandler)
            .ConfigureHttpClient(x => x.BaseAddress = new("http://ddragon.leagueoflegends.com/"))
            ;

        services
            .AddHttpClient(Config.ClientApiHttpClientName);

        services
            .AddSingleton<ILeagueConnectionSettingsProvider, LeagueConnectionSettingsProvider>()
            .AddSingleton<ILeagueClientApiConnector>(svc =>
                new LeagueClientApiConnector(
                    svc.GetRequiredService<IHttpClientFactory>().CreateClient(Config.ClientApiHttpClientName),
                    svc.GetRequiredService<ILeagueConnectionSettingsProvider>()))
            .AddSingleton<ILeagueClientWebSocketConnector, LeagueClientWebSocketConnector>()
            .AddSingleton<IDataDragonApiConnector, DataDragonApiConnector>()
            .AddSingleton<ILeagueLiveApiConnector, LeagueLiveApiConnector>()
            ;

        services
            // General
            .AddScoped<IGameflowService, GameflowService>()

            // Post Game
            .AddScoped<IHonorService, HonorService>()
            ;

        return services;
    }
}
