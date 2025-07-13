using Eshop.Domain.Entities;

namespace EShop.Application.Contracts.Repositories;
public interface IProductRepository
{
    // عملیات پایه
    Task<Product?> GetByIdAsync(Guid id);
    Task UpdateAsync(Product product);

    // لیست محصولات با فیلترهای ضروری
    Task<IReadOnlyList<Product>> GetListAsync(
        bool includeLocked = false,
        bool includeSold = false,
        CancellationToken cancellationToken = default);

    Task MarkProductAsSold(Guid id);
}
