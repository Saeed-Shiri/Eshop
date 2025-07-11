using Eshop.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Eshop.Domain.Entities;
public class BasketItem(Guid productId, int quantity, decimal priceAtAddition)
{
    public Guid ProductId { get; } = productId;
    public int Quantity { get; } = quantity > 0 ? quantity : throw new BasketQuantityNegativeException();
    public decimal PriceAtAddition { get; } = priceAtAddition > 0 ? priceAtAddition : throw new BasketPriceNegativeException();

}
