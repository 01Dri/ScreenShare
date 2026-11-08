using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ScreenShare.App.ViewModels;
using System.Threading.Tasks;

namespace ScreenShare.App.Views;

public partial class RoomWindow : Window
{
    public RoomWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (DataContext is RoomViewModel { HasLeft: false } model)
        {
            _ = model.LeaveAsync();
        }

        base.OnClosing(e);
    }

    private async void OnCopyJoinKey(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not RoomViewModel { CurrentRoom.JoinKey: { Length: > 0 } key })
        {
            return;
        }

        if (Clipboard is { } clipboard)
        {
            using var transfer = new DataTransfer();
            transfer.Add(DataTransferItem.CreateText(key));

            await clipboard.SetDataAsync(transfer);

            if (sender is Button button)
            {
                var original = button.Content;
                button.Content = "Copiado";
                await Task.Delay(1500);
                button.Content = original;
            }
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.F11)
        {
            WindowState = WindowState == WindowState.FullScreen
                ? WindowState.Normal
                : WindowState.FullScreen;
            e.Handled = true;
        }
        else if (e.Key == Key.Escape && WindowState == WindowState.FullScreen)
        {
            WindowState = WindowState.Normal;
            e.Handled = true;
        }

        base.OnKeyDown(e);
    }
}
