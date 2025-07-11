

using Eshop.Domain.Common;
using Eshop.Domain.Exceptions;
using Eshop.Domain.Interfaces.Services;
using System;

namespace Eshop.Domain.Entities;
public class Basket : AggregateRoot
{
    
    public Guid UserId { get; }

    
    private readonly List<BasketItem> _items = [];
    public IReadOnlyList<BasketItem> Items => _items.AsReadOnly();

    public DateTime CreatedAt { get; }

    public DateTime ExpiresAt { get; }

    public decimal TotalPrice => _items.Sum(x => x.PriceAtAddition * x.Quantity);

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    private readonly IProductLockService _productLockService;

    public Basket(Guid id, Guid userId, IProductLockService productLockService) : base(id)
    {
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddMinutes(10);
        _productLockService = productLockService;
    }

    public async Task AddItemAsync(BasketItem item)
    {
        if (item == null)
            throw new BasketItemNullException();

        if (IsExpired)
            throw new BasketExpiredException();

        await _productLockService.LockProductAsync(
            item.ProductId,
            TimeSpan.FromMinutes(10));

        var existingItem = _items.FirstOrDefault(x => x.ProductId == item.ProductId);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + item.Quantity;
            _items.Remove(existingItem);
            _items.Add(new BasketItem(
                item.ProductId, 
                newQuantity, 
                item.PriceAtAddition)
                );
        }
        else 
        {
            _items.Add(item);
        }

        
    }

    public async Task RemoveItem(Guid productId, int quantity = 1)
    {
        var item = _items.FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
            return;

        if (item.Quantity <= quantity)
        {
            _items.Remove(item);

            // آزادسازی قفل اگر آیتم کاملاً حذف شد
            await _productLockService.UnlockProductAsync(productId);
        }
        else
        {
            _items.Remove(item);
            _items.Add(new BasketItem(
                item.ProductId,
                item.Quantity - quantity,
                item.PriceAtAddition
            ));
        }

    }

    // تخلیه سبد خرید
    public async Task ClearAsync()
    {
        // آزادسازی قفل همه محصولات
        foreach (var item in _items)
        {
            await _productLockService.UnlockProductAsync(item.ProductId);
        }
        _items.Clear();
    }

}
