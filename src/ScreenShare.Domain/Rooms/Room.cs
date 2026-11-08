using ScreenShare.Domain.Users;

namespace ScreenShare.Domain.Rooms
{
    public sealed class Room
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; init; } = string.Empty;
        public string JoinKey { init;  get; } = string.Empty;
        public User Owner { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public List<User> Users { get; init; } = [];

        public string BuildDatabaseKey()
        {
            return BuildDatabaseKey(JoinKey);
        }

        public static string BuildDatabaseKey(string joinKey)
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


        private string BuildJoinKey()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";

            var random = Random.Shared;

            var prefix = new string(
                Enumerable.Range(0, 3)
                    .Select(_ => letters[random.Next(letters.Length)])
                    .ToArray());

            var firstPart = random.Next(100, 1000);
            var secondPart = random.Next(100, 1000);
            return $"{prefix}-{firstPart}-{secondPart}";
        }
    }
}
