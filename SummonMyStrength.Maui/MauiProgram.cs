using MudBlazor.Services;
using SummonMyStrength.Api.Setup;
using SummonMyStrength.Maui.Components.Framework;
using SummonMyStrength.Maui.ComponentsOld.Common.DragDrop;
using SummonMyStrength.Maui.Data;
using SummonMyStrength.Maui.Services;

namespace SummonMyStrength.Maui;

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

        builder.Services.AddLeagueOfLegendsServices();
        builder.Services.AddSingleton<IUserSettingsService, UserSettingsService>();
        builder.Services.AddSingleton<IComponentSettingsService, ComponentSettingsService>();
        builder.Services.AddSingleton<IDragDropService, DragDropService>();
        builder.Services.AddTransient<IChampSelectSessionAbstractor, ChampSelectSessionAbstractor>();
        builder.Services.AddMudServices();

        var app = builder.Build();

        return app;
    }
}