using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenShare.Domain;
using ScreenShare.Domain.Rooms;
using ScreenShare.Domain.Users;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScreenShare.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private const int MinUsernameLength = 3;
    private const int MinRoomNameLength = 3;
    private const int MaxRoomNameLength = 50;

    private static readonly Regex JoinKeyPattern =
        new(@"^[A-Z]{3}-\d{3}-\d{3}$", RegexOptions.Compiled);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUsernameError))]
    [NotifyCanExecuteChangedFor(nameof(CreateRoomCommand))]
    [NotifyCanExecuteChangedFor(nameof(JoinRoomCommand))]
    public partial string Username { get; set; } = string.Empty;

    /// <summary>Criar uma sala (pede nome) ou entrar em uma existente (pede o código).</summary>
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
    [NotifyPropertyChangedFor(nameof(HasRoomKeyError))]
    [NotifyPropertyChangedFor(nameof(RoomKeyError))]
    [NotifyCanExecuteChangedFor(nameof(JoinRoomCommand))]
    public partial string RoomKey { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasJoinError))]
    public partial string? JoinError { get; set; }

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

    public bool HasRoomKeyError => RoomKey.Length > 0 && !IsRoomKeyValid;

    public string RoomKeyError => "Código inválido. Use o formato ABC-123-456.";

    public bool HasJoinError => !string.IsNullOrEmpty(JoinError);

    private bool IsUsernameValid => Username.Trim().Length >= MinUsernameLength;

    private bool IsRoomNameValid => RoomName.Trim().Length is >= MinRoomNameLength and <= MaxRoomNameLength;

    private bool IsRoomKeyValid => JoinKeyPattern.IsMatch(Room.NormalizeJoinKey(RoomKey));

    private bool CanCreateRoom => IsUsernameValid && IsRoomNameValid;

    private bool CanJoinRoom => IsUsernameValid && IsRoomKeyValid;

    private Room? CurrentRoom { get; set; }

    /// <summary>Usuário da sessão (dono ao criar, convidado ao entrar).</summary>
    public User? CurrentUser { get; private set; }

    /// <summary>Disparado quando a sala é criada ou acessada, para a UI abrir a tela da sala.</summary>
    public event EventHandler<Room>? RoomEntered;

    private readonly IRedisService _redisDatabase;

    public MainViewModel(IRedisService redisDatabase)
    {
        _redisDatabase = redisDatabase;
    }

    partial void OnRoomKeyChanged(string value) => JoinError = null;

    [RelayCommand(CanExecute = nameof(CanCreateRoom))]
    private async Task CreateRoomAsync()
    {
        if (CurrentRoom != null)
        {
            RoomEntered?.Invoke(this, CurrentRoom);
            return;
        }

        var owner = new User(Username.Trim());
        var room = new Room(RoomName, owner);   
        owner.CurrentRoom = room;

        await _redisDatabase.SaveAsync(room.BuildKey(), room);

        CurrentUser = owner;
        CurrentRoom = room;
        RoomEntered?.Invoke(this, room);
    }

    [RelayCommand(CanExecute = nameof(CanJoinRoom))]
    private async Task JoinRoomAsync()
    {
        JoinError = null;

        var databaseKey = Room.BuildKey(RoomKey);

        try
        {
            var room = await _redisDatabase.GetAsync<Room>(databaseKey);
            if (room is null)
            {
                JoinError = "Sala não encontrada. Confira o código.";
                return;
            }

            var user = CurrentUser ?? new User(Username.Trim());

            var alreadyInRoom = room.Users.Any(existing => existing.Id == user.Id);

            if (!alreadyInRoom)
            {
                room.Users.Add(user);
                await _redisDatabase.SaveAsync(databaseKey, room);
            }

            user.CurrentRoom = room;
            CurrentUser = user;
            CurrentRoom = room;

            RoomEntered?.Invoke(this, room);
        }
        catch (Exception ex )
        {
            JoinError = "Não foi possível entrar na sala. Tente novamente.";
            Native.OS.LogException(ex);
        }
    }
}
