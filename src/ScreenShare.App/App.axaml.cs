using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ScreenShare.App.ViewModels;
using ScreenShare.App.Views;
using ScreenShare.Domain.Rooms;
using ScreenShare.Domain.Users;
using ScreenShare.IoC.Extensions;

namespace ScreenShare.App;

public partial class App : Avalonia.Application
{
    private IServiceProvider? _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _services = BuildMainViewModelContainerIoC();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = OpenMainWindow(desktop);
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

    private Window OpenMainWindow(IClassicDesktopStyleApplicationLifetime desktop)
    {
        var vm = _services!.GetRequiredService<MainViewModel>();
        var window = new MainWindow
        {
            DataContext = vm
        };

        vm.RoomEntered += (_, room) => OpenRoomWindow(desktop, window, room, vm.CurrentUser);

        window.Show();
        desktop.MainWindow = window;
        return window;
    }

    private void OpenRoomWindow(IClassicDesktopStyleApplicationLifetime desktop, Window mainWindow, Room room, User? currentUser)
    {
        var vm = _services!.GetRequiredService<RoomViewModel>();
        vm.CurrentRoom = room;
        vm.CurrentUser = currentUser;

        var window = new RoomWindow
        {
            DataContext = vm
        };

        vm.LeaveRequested += (_, _) =>
        {
            var main = OpenMainWindow(desktop);
            desktop.MainWindow = main;
            window.Close();
        };

        window.Show();
        desktop.MainWindow = window;
        mainWindow.Close();
    }

    private ServiceProvider BuildMainViewModelContainerIoC()
    {
        var collection = new ServiceCollection();
        collection.ConfigureServices();
        collection.AddTransient<MainViewModel>();
        collection.AddTransient<RoomViewModel>();
        return collection.BuildServiceProvider();
    }
}
