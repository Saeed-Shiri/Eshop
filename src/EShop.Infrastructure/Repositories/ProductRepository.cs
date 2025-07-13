
using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using EShop.Application.Contracts.Repositories;
using EShop.Application.Contracts.Services;
using EShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace EShop.Infrastructure.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    private readonly IProductLockService _productLockService;

    public ProductRepository(
        AppDbContext context,
        IProductLockService productLockService)
    {
        _context = context;
        _productLockService = productLockService;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .FindAsync(id);
    }

    public async Task<IReadOnlyList<Product>> GetListAsync(
        bool includeLocked = false,
        bool includeSold = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();

        // فیلتر بر اساس وضعیت محصولات
        if (!includeLocked || !includeSold)
        {
            query = query.Where(x =>
                (includeLocked || x.Status != ProductStatus.Locked) &&
                (includeSold || x.Status != ProductStatus.Sold));
        }

        var products = await query
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // بررسی وضعیت قفل برای محصولات
        foreach (var product in products.Where(x => x.Status == ProductStatus.Available))
        {
            if ((await _productLockService.GetLockExpiryAsync(product.Id)) != null)
            {
                product.MarkAsLocked();
            }
        }

        return products;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products
            .Update(product);

        await _context.SaveChangesAsync();
    }
}
