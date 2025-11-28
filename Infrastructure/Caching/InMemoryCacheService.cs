using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Infrastructure.Caching;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

    public InMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var value = _memoryCache.Get<string>(key);

        if (string.IsNullOrEmpty(value))
            return Task.FromResult<T?>(null);

        var deserializedValue = JsonSerializer.Deserialize<T>(value);
        return Task.FromResult(deserializedValue);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var serializedValue = JsonSerializer.Serialize(value);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
        };

        _memoryCache.Set(key, serializedValue, cacheOptions);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var exists = _memoryCache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }
}