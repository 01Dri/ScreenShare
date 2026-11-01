using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenShare.App.Models;

namespace ScreenShare.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private const int MinUsernameLength = 3;
    private const int MinRoomNameLength = 3;
    private const int MaxRoomNameLength = 50;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUsernameError))]
    [NotifyCanExecuteChangedFor(nameof(CreateRoomCommand))]
    [NotifyCanExecuteChangedFor(nameof(JoinRoomCommand))]
    public partial string Username { get; set; } = string.Empty;

    /// <summary>Criar uma sala (pede nome) ou entrar em uma existente (pede o ID).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCreateSelected))]
    [NotifyPropertyChangedFor(nameof(IsJoinSelected))]
    [NotifyPropertyChangedFor(nameof(IsCreating))]
    public partial RoomMode SelectedMode { get; set; } = RoomMode.Create;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasRoomNameError))]
    [NotifyPropertyChangedFor(nameof(RoomNameError))]
    [NotifyCanExecuteChangedFor(nameof(CreateRoomCommand))]
    public partial string RoomName { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(JoinRoomCommand))]
    public partial string RoomId { get; set; } = string.Empty;

    public bool IsCreating => SelectedMode == RoomMode.Create;

    /// <summary>Espelho do modo para o segmento "Criar sala".</summary>
    public bool IsCreateSelected
    {
        get => SelectedMode == RoomMode.Create;
        set
        {
            if (value)
            {
                SelectedMode = RoomMode.Create;
            }
        }
    }

    /// <summary>Espelho do modo para o segmento "Entrar na sala".</summary>
    public bool IsJoinSelected
    {
        get => SelectedMode == RoomMode.Join;
        set
        {
            if (value)
            {
                SelectedMode = RoomMode.Join;
            }
        }
    }

    public bool HasUsernameError => Username.Length > 0 && !IsUsernameValid;

    public string UsernameError => $"O nome precisa ter ao menos {MinUsernameLength} caracteres.";

    public bool HasRoomNameError => RoomName.Length > 0 && !IsRoomNameValid;

    public string RoomNameError => RoomName.Trim().Length > MaxRoomNameLength
        ? $"O nome da sala pode ter no máximo {MaxRoomNameLength} caracteres."
        : $"O nome da sala precisa ter ao menos {MinRoomNameLength} caracteres.";

    private bool IsUsernameValid => Username.Trim().Length >= MinUsernameLength;

    private bool IsRoomNameValid => RoomName.Trim().Length is >= MinRoomNameLength and <= MaxRoomNameLength;

    private bool CanCreateRoom => IsUsernameValid && IsRoomNameValid;

    private bool CanJoinRoom => IsUsernameValid && !string.IsNullOrWhiteSpace(RoomId);

    // TODO: criar a sala privada com o nome informado e abrir a transmissão.
    [RelayCommand(CanExecute = nameof(CanCreateRoom))]
    private void CreateRoom()
    {
    }

    // TODO: entrar na sala pelo ID informado e abrir a transmissão.
    [RelayCommand(CanExecute = nameof(CanJoinRoom))]
    private void JoinRoom()
    {
    }
}
