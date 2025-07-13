

using EShop.Application.Contracts.Services;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace EShop.Infrastructure.Services.Cache;
public class RedisProductLockService : IProductLockService
{
    private readonly IDistributedCache _cache;
    private readonly IDatabase _redis;
    private const string KeyPrefix = "lock:product:";

    public RedisProductLockService(
        IDistributedCache cache,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _cache = cache;
        _redis = connectionMultiplexer.GetDatabase();
    }

    public async Task<DateTime?> GetLockExpiryAsync(Guid productId)
    {
        var key = GetKey(productId);

        var ttl = await _redis.KeyTimeToLiveAsync(key);

        return ttl.HasValue 
            ? DateTime.UtcNow.Add(ttl.Value) 
            : null;
    }

    public async Task LockProductAsync(Guid productId, TimeSpan duration)
    {
        var key = GetKey(productId);
        var value = "locked";

        await _cache
            .SetStringAsync(
            key,
            value,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duration
            });
    }

    public async Task UnlockProductAsync(Guid productId)
    {
        var key = GetKey(productId);
        await _cache.RemoveAsync(key);
    }

    private static string GetKey(Guid productId)
    {
        return $"{KeyPrefix}{productId:N}";
    }
}
