using Eshop.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;


namespace Eshop.Domain.Entities;

public class BasketItem
{
    [JsonConstructor]
    public BasketItem(
        Guid productId,
        int quantity, 
        decimal priceAtAddition)
    {
        ProductId = productId;
        Quantity = quantity > 0 ? quantity : throw new BasketQuantityNegativeException();
        PriceAtAddition = priceAtAddition > 0 ? priceAtAddition : throw new BasketPriceNegativeException();
    }

    public Guid ProductId { get; }
    public int Quantity { get; }
    public decimal PriceAtAddition { get; }
}
