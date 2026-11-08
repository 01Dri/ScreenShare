using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenShare.Domain;
using ScreenShare.Domain.Rooms;
using ScreenShare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ScreenShare.App.ViewModels;

public sealed partial class RoomViewModel : ViewModelBase
{
    private readonly IRedisService _redisService;

    public RoomViewModel(IRedisService redisService)
    {
        _redisService = redisService;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RoomName))]
    [NotifyPropertyChangedFor(nameof(ParticipantsLabel))]
    public partial Room? CurrentRoom { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LiveButtonText))]
    [NotifyPropertyChangedFor(nameof(StageTitle))]
    [NotifyPropertyChangedFor(nameof(StageDescription))]
    public partial bool IsLive { get; set; }

    /// <summary>Usuário que está usando a tela (dono ou convidado).</summary>
    public User? CurrentUser { get; set; }

    public bool IsOwner => CurrentUser is { } user
        && CurrentRoom is { } room
        && room.Owner.Id == user.Id;

    public string RoomName => CurrentRoom?.Name ?? string.Empty;

    public string LiveButtonText => IsLive ? "Encerrar live" : "Iniciar live";

    public string StageTitle => IsLive ? "Você está ao vivo" : "Pronto para compartilhar";

    public string StageDescription => IsLive
        ? "Sua tela está sendo compartilhada com a sala."
        : "Clique em Iniciar live para começar a transmitir sua tela.";

    public ObservableCollection<RoomUserViewModel> Users { get; } = [];

    public string ParticipantsLabel => Users.Count == 1
        ? "1 pessoa na sala"
        : $"{Users.Count} pessoas na sala";

    /// <summary>Pedido para fechar esta janela e voltar para a tela inicial.</summary>
    public event EventHandler? LeaveRequested;

    public bool HasLeft { get; private set; }

    partial void OnCurrentRoomChanged(Room? value) => RefreshParticipants();

    public void RefreshParticipants()
    {
        Users.Clear();

        if (CurrentRoom is not { } room)
        {
            OnPropertyChanged(nameof(ParticipantsLabel));
            return;
        }

        IReadOnlyList<User> participants = room.Users.Count > 0
            ? room.Users
            : new List<User> { room.Owner };

        foreach (var user in participants)
        {
            Users.Add(new RoomUserViewModel(user, isOwner: user.Id == room.Owner.Id));
        }

        OnPropertyChanged(nameof(ParticipantsLabel));
    }

    // TODO: iniciar/encerrar a transmissão de tela de verdade.
    [RelayCommand]
    private void ToggleLive() => IsLive = !IsLive;

    [RelayCommand]
    private async Task LeaveRoomAsync()
    {
        await LeaveAsync();
        LeaveRequested?.Invoke(this, EventArgs.Empty);
    }

    public async Task LeaveAsync()
    {
        if (HasLeft)
        {
            return;
        }

        HasLeft = true;

        if (CurrentRoom is not { } room)
        {
            return;
        }

        if (IsOwner)
        {
            await _redisService.DeleteAsync(room.BuildKey());
            return;
        }

        if (CurrentUser is { } user)
        {
            room.Users.RemoveAll(existing => existing.Id == user.Id);
            await _redisService.SaveAsync(room.BuildKey(), room);
        }
    }
}
