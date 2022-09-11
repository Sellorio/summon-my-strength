using MudBlazor.Services;
using SummonMyStrength.Api;
using SummonMyStrength.Maui.Services;

namespace SummonMyStrength.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(typeof(MauiProgram).Assembly.Location));

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
		    builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            var leagueClient = new LeagueClient();
            var champSelectSessionAccessor = new ChampSelectSessionAccessor(leagueClient);
            builder.Services.AddSingleton(leagueClient);
            builder.Services.AddSingleton<IChampSelectSessionAccessor>(champSelectSessionAccessor);
            builder.Services.AddSingleton<IRuneSetService>(new RuneSetService(leagueClient));
            builder.Services.AddSingleton<IPickBanService>(new PickBanService(leagueClient, champSelectSessionAccessor));
            builder.Services.AddSingleton<IGameInfoAccessor>(new GameInfoAccessor(leagueClient));
            builder.Services.AddMudServices();

            return builder.Build();
        }
    }
}