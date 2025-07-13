
using Eshop.Domain.Enums;
using Eshop.Domain.Exceptions;

namespace Eshop.Domain.Entities;
public class Product(
    string name,
    decimal price)
{
    public Guid Id { get; }
    public string Name { get; private set; } = name ?? throw new ProducNameNullException();
    public decimal Price { get; private set; } = price > 0 ? price : throw new ProductPriceNegativeException();
    public ProductStatus Status { get; private set; } = ProductStatus.Available;

    public void MarkAsLocked()
    {
        if (Status != ProductStatus.Available)
            throw new ProductNotAvailabeException(Id);

        Status = ProductStatus.Locked;
    }

    public void MarkAsSold()
    {
        if (Status != ProductStatus.Locked)
            throw new ProductNotLockedException(Id);

        Status = ProductStatus.Sold;
    }

    public void ReleaseLock()
    {
        if(Status == ProductStatus.Locked)
            Status = ProductStatus.Sold;

        else
            throw new ProductReleaseLockException(Id);
    }
}
