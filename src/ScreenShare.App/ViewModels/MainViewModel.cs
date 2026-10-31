using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ScreenShare.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private const int MinUsernameLength = 3;
    private const int MinRoomNameLength = 3;
    private const int MaxRoomNameLength = 50;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUsernameError))]
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
    [NotifyPropertyChangedFor(nameof(HasRoomNameError))]
    [NotifyPropertyChangedFor(nameof(RoomNameError))]
    [NotifyPropertyChangedFor(nameof(CanContinue))]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    public partial string RoomName { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanContinue))]
    [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
    public partial string RoomId { get; set; } = string.Empty;

    public bool IsJoiningRoom => !IsCreatingRoom;

    public bool HasUsernameError => Username.Length > 0 && !IsUsernameValid;

    public string UsernameError => $"O nome de usuário precisa ter ao menos {MinUsernameLength} caracteres.";

    public bool HasRoomNameError => RoomName.Length > 0 && !IsRoomNameValid;

    public string RoomNameError => RoomName.Trim().Length > MaxRoomNameLength
        ? $"O nome da sala pode ter no máximo {MaxRoomNameLength} caracteres."
        : $"O nome da sala precisa ter ao menos {MinRoomNameLength} caracteres.";

    public bool CanContinue => IsUsernameValid
        && (IsCreatingRoom ? IsRoomNameValid : !string.IsNullOrWhiteSpace(RoomId));

    private bool IsUsernameValid => Username.Trim().Length >= MinUsernameLength;

    private bool IsRoomNameValid => RoomName.Trim().Length is >= MinRoomNameLength and <= MaxRoomNameLength;

    // TODO: criar a sala privada (nome) ou entrar na sala (id) usando o nome de usuário.
    //       Hoje todas as salas são privadas; a visibilidade deixou de ser uma opção do usuário.
    [RelayCommand(CanExecute = nameof(CanContinue))]
    private void Continue()
    {
    }
}
