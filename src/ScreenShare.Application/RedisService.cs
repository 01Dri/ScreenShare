using ScreenShare.Domain;
using StackExchange.Redis;
using System.Text.Json;

namespace ScreenShare.Application;

public sealed class RedisService : IRedisService
{
    private readonly IDatabase _database;

    public RedisService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task SaveAsync<T>(string key, T item) where T : class
    {
        var value = JsonSerializer.Serialize(item);
        await _database.StringSetAsync(key, value);
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await _database.StringDeleteAsync(key, ValueCondition.Exists);
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var value = await _database.StringGetAsync(key);

        return value.HasValue
            ? JsonSerializer.Deserialize<T>((string)value!)
            : default;
    }
}