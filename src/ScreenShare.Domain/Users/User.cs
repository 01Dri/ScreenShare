using ScreenShare.Domain.Rooms;
using System.Text.Json.Serialization;

namespace ScreenShare.Domain.Users
{
    public sealed class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public Room? CurrentRoom { get; set; }

        public DateTime CreatedAt { get; } = DateTime.UtcNow;

        public User(string username)
        {
            Name = username;
        }
    }
}
