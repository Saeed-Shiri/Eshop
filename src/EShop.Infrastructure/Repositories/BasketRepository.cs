

using Eshop.Domain.Entities;
using EShop.Application.Contracts.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;


namespace EShop.Infrastructure.Repositories;
public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _cache;
    private const string BasketPrefix = "basket:";

    public BasketRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task DeleteBasketAsync(Guid userId)
    {
        var key = GetKey(userId);

        await _cache
            .RemoveAsync($"basket:{userId}");
    }

    public async Task<Basket?> GetBasketAsync(Guid userId)
    {
        var key = GetKey(userId);
        var data = await _cache.GetStringAsync(key);

        if (string.IsNullOrWhiteSpace(data))
            return null;

        return JsonSerializer.Deserialize<Basket>(data);
    }

    public async Task UpdateBasketAsync(Basket basket)
    {
        var key = GetKey(basket.UserId);
        var data = JsonSerializer.Serialize(basket);

        await _cache
            .SetStringAsync(
            key,
            data,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
    }

    private static string GetKey(Guid userId)
    {
        return $"{BasketPrefix}{userId:N}";
    }
}
