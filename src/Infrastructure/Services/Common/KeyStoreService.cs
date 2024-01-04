using Application.Services;
using Newtonsoft.Json;
using Selise.Ecap.Infrastructure;

namespace Infrastructure.Services.Common
{
    public class KeyStoreService : IKeyStoreService
    {
        private readonly IKeyStore _keyStore;

        public KeyStoreService(IKeyStore keyStore)
        {
            _keyStore = keyStore;
        }

        public Task AddValueAsync<T>(
            string key,
            T value,
            int expiryTimeSeconds)
            where T : class
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            return _keyStore.AddKeyWithExprityAsync(key, jsonValue, expiryTimeSeconds);
        }

        public async Task<T> GetValueAsync<T>(string key)
            where T : class
        {
            var value = await _keyStore.GetValueAsync(key);
            return string.IsNullOrWhiteSpace(value)
                ? null
                : JsonConvert.DeserializeObject<T>(value);
        }

        public Task DeleteValueAsync(string key)
        {
            return _keyStore.RemoveKeyAsync(key);
        }
    }
}
