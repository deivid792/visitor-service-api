namespace VisitorService.aplication.Interface
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string Key);
        Task SetAsync<T>(string Key, T value, TimeSpan? expiration = null);

        Task RemoveAsync(string Key);
    }
}

