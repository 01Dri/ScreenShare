using ScreenShare.Domain.Users;

namespace ScreenShare.App.ViewModels;

/// <summary>Linha da lista de participantes da sala.</summary>
public sealed class RoomUserViewModel
{
    public RoomUserViewModel(User user, bool isOwner)
    {
        Name = string.IsNullOrWhiteSpace(user.Name) ? "Sem nome" : user.Name.Trim();
        Role = isOwner ? "Anfitrião" : "Convidado";
        Initial = Name[..1].ToUpperInvariant();
    }

    public string Name { get; }

    public string Role { get; }

    public string Initial { get; }
}
