using SummonMyStrength.Maui.Data;

namespace SummonMyStrength.Maui;

public partial class App : Application
{
    public App(IUserSettingsService userSettingsService)
    {
        InitializeComponent();

        var userSettings = userSettingsService.GetSettings();
        
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
#if WINDOWS
            var mauiWindow = handler.VirtualView;
            var nativeWindow = handler.PlatformView;

            nativeWindow.Activate();
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new Windows.Graphics.SizeInt32(userSettings.WindowWidth ?? 630, userSettings.WindowHeight ?? 700));
            
            if (userSettings.WindowX != null && userSettings.WindowY != null)
            {
                appWindow.Move(new(userSettings.WindowX.Value, userSettings.WindowY.Value));
            }

            appWindow.Changed += (x, y) =>
            {
                if (y.DidSizeChange)
                {
                    userSettings.WindowWidth = appWindow.Size.Width;
                    userSettings.WindowHeight = appWindow.Size.Height;
                }

                if (y.DidPositionChange)
                {
                    userSettings.WindowX = appWindow.Position.X;
                    userSettings.WindowY = appWindow.Position.Y;
                }

                if (y.DidSizeChange || y.DidPositionChange)
                {
                    _ = userSettingsService.SaveSettingsAsync();
                }
            };
#endif
        });
        
        MainPage = new MainPage();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        if (window != null)
        {
            window.Title = "Summon My Strength";
        }

        return window;
    }
}