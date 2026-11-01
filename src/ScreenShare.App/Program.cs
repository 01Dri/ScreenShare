using Avalonia;
using System;
using System.Threading.Tasks;
using Avalonia.Media;

namespace ScreenShare.App;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static void Main(string[] args)
    {
        Native.OS.SetupBasicDirectories();
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            Native.OS.LogException(e.ExceptionObject as Exception);
        };

        TaskScheduler.UnobservedTaskException += (_, e) => { e.SetObserved(); };

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Native.OS.LogException(ex);
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = AppBuilder.Configure<App>();
        builder.UsePlatformDetect();
        builder.LogToTrace();
        builder.WithInterFont();
#if DEBUG
        builder.WithDeveloperTools();
#endif
        builder.With(new FontManagerOptions()
        {
            DefaultFamilyName = "fonts:Inter#Inter"
        });

        Native.OS.SetupApp(builder);
        return builder;
    }
}