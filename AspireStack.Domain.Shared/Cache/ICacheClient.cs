using AspireStack.Domain.Shared.Cache;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspireStack.Domain.Cache
{
    public interface ICacheClient
    {
        T? Get<T>(string key);
        Task<T?> GetAsync<T>(string key, CancellationToken token = default);
        void Refresh(string key);
        Task RefreshAsync(string key, CancellationToken token = default);
        void Remove(string key);
        Task RemoveAsync(string key, CancellationToken token = default);
        void Set<T>(string key, T value, CacheEntryOptions options);
        void Set<T>(string key, T value);
        void Set<T>(string key, T value, TimeSpan expirationTime);
        Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken token = default);
        Task SetAsync<T>(string key, T value, CancellationToken token = default);
        Task SetAsync<T>(string key, T value, TimeSpan expirationTime, CancellationToken token = default);
        void SetString(string key, string value, TimeSpan expirationTime);
        void SetString(string key, string value);
        Task SetStringAsync<T>(string key, string value, TimeSpan expirationTime, CancellationToken token = default);
        Task SetStringAsync<T>(string key, string value, CancellationToken token = default);
    }
}
