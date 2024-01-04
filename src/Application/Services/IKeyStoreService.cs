
namespace Application.Services
{
    public interface IKeyStoreService
    {
        private const int ThreeDaysMilliseconds = 3 * 24 * 3600000;
        Task AddValueAsync<T>(
            string key,
            T value,
            int expiryTimeSeconds = ThreeDaysMilliseconds)
            where T : class;

        Task<T> GetValueAsync<T>(string key)
            where T : class;

        Task DeleteValueAsync(string key);
    }
}
