using AspireStack.Domain.Cache;
using AspireStack.Domain.Shared.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace AspireStack.Infrastructure.Cache
{
    internal class CacheClient : ICacheClient
    {
        private readonly IDistributedCache cache;
        private readonly ILogger<CacheClient> logger;

        public CacheClient(IDistributedCache cache, ILogger<CacheClient> logger)
        {
            this.cache = cache;
            this.logger = logger;
        }

        public string? GetString(string key)
        {
            return cache.GetString(key);
        }

        public async Task<string?> GetStringAsync(string key, CancellationToken token = default)
        {
            return await cache.GetStringAsync(key, token);
        }

        public T? Get<T>(string key)
        {
            var stringData = cache.GetString(key);
            if (stringData == null)
            {
                return default;
            }
            try
            {
                return JsonSerializer.Deserialize<T>(stringData);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to deserialize cache item with key {key}", key);
                return default;
            }
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
        {
            var stringData = await cache.GetStringAsync(key, token);
            if (stringData == null)
            {
                return default;
            }
            try
            {
                return JsonSerializer.Deserialize<T>(stringData);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to deserialize cache item with key {key}", key);
                return default;
            }
        }

        public void Refresh(string key)
        {
            cache.Refresh(key);
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            return cache.RefreshAsync(key, token);
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            return cache.RemoveAsync(key, token);
        }

        public void Set<T>(string key, T value, CacheEntryOptions options)
        {
            DistributedCacheEntryOptions distributedCacheEntryOptions = new()
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration
            };
            cache.SetString(key, JsonSerializer.Serialize(value), distributedCacheEntryOptions);
        }

        public Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken token = default)
        {
            DistributedCacheEntryOptions distributedCacheEntryOptions = new()
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration
            };
            return cache.SetStringAsync(key, JsonSerializer.Serialize(value), distributedCacheEntryOptions, token);
        }

        public void Set<T>(string key, T value)
        {
            cache.SetString(key, JsonSerializer.Serialize(value));
        }

        public Task SetAsync<T>(string key, T value, CancellationToken token = default)
        {
            return cache.SetStringAsync(key, JsonSerializer.Serialize(value), token);
        }

        public void Set<T>(string key, T value, TimeSpan expirationTime)
        {
            cache.SetString(key, JsonSerializer.Serialize(value), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime });
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expirationTime, CancellationToken token = default)
        {
            return cache.SetStringAsync(key, JsonSerializer.Serialize(value), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime }, token);
        }

        public void SetString(string key, string value)
        {
            cache.SetString(key, value);
        }

        public Task SetStringAsync<T>(string key, string value, CancellationToken token = default)
        {
            return cache.SetStringAsync(key, value, token);
        }

        public void SetString(string key, string value, TimeSpan expirationTime)
        {
            cache.SetString(key, value, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime });
        }

        public Task SetStringAsync<T>(string key, string value, TimeSpan expirationTime, CancellationToken token = default)
        {
            return cache.SetStringAsync(key, value, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime }, token);
        }
    }
}
