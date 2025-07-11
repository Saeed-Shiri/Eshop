

namespace Eshop.Domain.Exceptions;
public class BasketQuantityNegativeException() : DomainException("Quantity must be positive");

public class BasketPriceNegativeException() : DomainException("Price must be positive");

public class BasketItemNullException() : DomainException("Basket item cannot be null");

public class BasketNotFoundException(Guid userId) : DomainException($"Basket for user with id {userId} not found");
public class BasketExpiredException() : DomainException("Cannot add item to expired basket");