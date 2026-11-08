using ScreenShare.Domain.Users;
using System.Security.Cryptography;

namespace ScreenShare.Domain.Rooms
{
    public sealed class Room
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; init; } = string.Empty;
        public string JoinKey { init; get; } = string.Empty;
        public User Owner { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public List<User> Users { get; init; } = [];

        public string BuildKey()
        {
            return BuildKey(JoinKey);
        }

        public static string BuildKey(string joinKey)
        {
            return $"room:{NormalizeJoinKey(joinKey)}";
        }

        public static string NormalizeJoinKey(string joinKey)
        {
            return joinKey.Trim().ToUpperInvariant();
        }

        public Room(string name, User owner)
        {
            JoinKey = BuildJoinKey();
            Name = name;
            Owner = owner;
            Users.Add(owner);
        }

        /**
         * Temos no total 492.804.000 combinações de IDS, porém, colisões ainda podem ocorrer. Estudar boas praticas para evitar colisões.
        **/

        private string BuildJoinKey()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var prefix = RandomNumberGenerator.GetString(letters, 3);

            var firstPart = RandomNumberGenerator.GetInt32(100, 1000);
            var secondPart = RandomNumberGenerator.GetInt32(100, 1000);

            return $"{prefix}-{firstPart}-{secondPart}";
        }
    }
}
