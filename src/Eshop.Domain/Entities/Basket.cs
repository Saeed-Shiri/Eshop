

using Eshop.Domain.Common;
using Eshop.Domain.Exceptions;
using System.Text.Json.Serialization;


namespace Eshop.Domain.Entities;

public class Basket : AggregateRoot
{
    
    public Guid UserId { get; }



    [JsonInclude]
    public List<BasketItem> Items { get; private set; } = [];

    public DateTime CreatedAt { get; }

    public DateTime ExpiresAt { get; }

    public decimal TotalPrice => Items.Sum(x => x.PriceAtAddition * x.Quantity);

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;


    [JsonConstructor]
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


        var existingItem = Items.FirstOrDefault(x => x.ProductId == item.ProductId);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + item.Quantity;
            Items.Remove(existingItem);
            Items.Add(new BasketItem(
                item.ProductId, 
                newQuantity, 
                item.PriceAtAddition)
                );
        }
        else 
        {
            Items.Add(item);
        }

        
    }

    public void RemoveItem(Guid productId, int quantity = 1)
    {
        var item = Items.FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
            return;

        Items.Remove(item);

        if (item.Quantity > quantity)
        {
            Items.Add(new BasketItem(
                item.ProductId,
                item.Quantity - quantity,
                item.PriceAtAddition
            ));

        }

    }

    // تخلیه سبد خرید
    public void Clear()
    {

        Items.Clear();
    }

}
