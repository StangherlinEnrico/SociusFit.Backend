using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

    public RedisCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(cachedValue))
            return null;

        return JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var serializedValue = JsonSerializer.Serialize(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
        };

        await _distributedCache.SetStringAsync(key, serializedValue, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _distributedCache.GetStringAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(value);
    }
}