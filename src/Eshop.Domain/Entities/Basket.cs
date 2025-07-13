

using Eshop.Domain.Common;
using Eshop.Domain.Exceptions;


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


    public Basket(
        Guid id,
        Guid userId) : base(id)
    {
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddMinutes(10);
    }

    public Basket(Guid userId) : base()
    {
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddMinutes(10);
    }

    public void AddItem(BasketItem item)
    {
        if (item == null)
            throw new BasketItemNullException();

        if (IsExpired)
            throw new BasketExpiredException();


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

    public void RemoveItem(Guid productId, int quantity = 1)
    {
        var item = _items.FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
            return;

        _items.Remove(item);

        if (item.Quantity > quantity)
        {
            _items.Add(new BasketItem(
                item.ProductId,
                item.Quantity - quantity,
                item.PriceAtAddition
            ));

        }

    }

    // تخلیه سبد خرید
    public void Clear()
    {

        _items.Clear();
    }

}
