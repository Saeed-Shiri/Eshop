
using Eshop.Domain.Entities;

namespace Eshop.Domain.Interfaces.Repositories;
public interface IProductRepository
{
    // عملیات پایه
    Task<Product> GetByIdAsync(Guid id);
    Task UpdateAsync(Product product);

    // لیست محصولات با فیلترهای ضروری
    Task<IReadOnlyList<Product>> GetListAsync(
        bool includeLocked = false,
        bool includeSold = false);
}
