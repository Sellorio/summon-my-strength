namespace SummonMyStrength.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
#if WINDOWS
            var mauiWindow = handler.VirtualView;
            var nativeWindow = handler.PlatformView;

            nativeWindow.Activate();
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new Windows.Graphics.SizeInt32(DataStore.WindowWidth ?? 630, DataStore.WindowHeight ?? 700));
            
            if (DataStore.WindowX != null && DataStore.WindowY != null)
            {
                appWindow.Move(new(DataStore.WindowX.Value, DataStore.WindowY.Value));
            }

            appWindow.Changed += (x, y) =>
            {
                if (y.DidSizeChange)
                {
                    DataStore.WindowWidth = appWindow.Size.Width;
                    DataStore.WindowHeight = appWindow.Size.Height;
                }

                if (y.DidPositionChange)
                {
                    DataStore.WindowX = appWindow.Position.X;
                    DataStore.WindowY = appWindow.Position.Y;
                }

                if (y.DidSizeChange || y.DidPositionChange)
                {
                    _ = DataStore.SaveAsync();
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