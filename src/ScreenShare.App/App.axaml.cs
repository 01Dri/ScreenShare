using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ScreenShare.App.ViewModels;
using ScreenShare.App.Views;
using ScreenShare.IoC.Extensions;

namespace ScreenShare.App;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = BuildMainViewModelContainerIoC();
        var vm = services.GetRequiredService<MainViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };
        }
        //else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        //{
        //    singleViewPlatform.MainView = new MainView
        //    {
        //        DataContext = vm
        //    };
        //}

        base.OnFrameworkInitializationCompleted();
    }

    private ServiceProvider BuildMainViewModelContainerIoC()
    {
        var collection = new ServiceCollection();
        collection.ConfigureServices();
        collection.AddTransient<MainViewModel>();
        return collection.BuildServiceProvider();
    }
}