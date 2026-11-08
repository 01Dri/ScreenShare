namespace ScreenShare.Domain
{
    public interface IRedisService
    {
        Task SaveAsync<T>(string key, T item) where T : class;
        Task<bool> DeleteAsync(string key);

        Task<T?> GetAsync<T>(string key) where T : class;
    }
}
