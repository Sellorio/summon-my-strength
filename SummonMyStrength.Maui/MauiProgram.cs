using MudBlazor.Services;
using SummonMyStrength.Api.Setup;
using SummonMyStrength.Maui.Components.Framework;
using SummonMyStrength.Maui.Components.Common.DragDrop;
using SummonMyStrength.Maui.Data;
using SummonMyStrength.Maui.Services;

namespace SummonMyStrength.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

#if WINDOWS
        ConfigureWebView2UserDataFolder();
#endif
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

        builder.Services.AddLeagueOfLegendsServices();
        builder.Services.AddSingleton<IUserSettingsService, UserSettingsService>();
        builder.Services.AddSingleton<IComponentSettingsService, ComponentSettingsService>();
        builder.Services.AddSingleton<IDragDropService, DragDropService>();
        builder.Services.AddTransient<IChampSelectSessionAbstractor, ChampSelectSessionAbstractor>();
        builder.Services.AddMudServices();

        var app = builder.Build();

        return app;
    }

#if WINDOWS
    private static void ConfigureWebView2UserDataFolder()
    {
        var webView2UserDataFolder = Path.Combine(Path.GetTempPath(), "Summon My Strength", "WebView2");
        Directory.CreateDirectory(webView2UserDataFolder);
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", webView2UserDataFolder);
    }
#endif
}