using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ScreenShare.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanContinue))]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    public partial string Username { get; set; } = string.Empty;

    /// <summary><c>true</c> para criar uma sala; <c>false</c> para entrar em uma existente.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsJoiningRoom))]
    [NotifyPropertyChangedFor(nameof(CanContinue))]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    public partial bool IsCreatingRoom { get; set; } = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanContinue))]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    public partial string RoomName { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanContinue))]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    public partial string RoomId { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsPublicRoom { get; set; } = true;

    public bool IsJoiningRoom => !IsCreatingRoom;

    public bool CanContinue => !string.IsNullOrWhiteSpace(Username)
        && (IsCreatingRoom
            ? !string.IsNullOrWhiteSpace(RoomName)
            : !string.IsNullOrWhiteSpace(RoomId));

    // TODO: criar a sala (nome + visibilidade) ou entrar na sala (id) usando o nome de usuário.
    [RelayCommand(CanExecute = nameof(CanContinue))]
    private void Continue()
    {
    }
}
