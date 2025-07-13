

using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using EShop.Application.Contracts.Repositories;
using EShop.Application.Contracts.Services;
using EShop.Application.Features.ProductFetures.Dtos;
using FluentResults;
using MediatR;

namespace EShop.Application.Features.ProductFetures.Queries.GetProducts;
public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<IReadOnlyList<ProductStatusDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductLockService _productLockService;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        IProductLockService productLockService)
    {
        _productRepository = productRepository;
        _productLockService = productLockService;
    }

    public async Task<Result<IReadOnlyList<ProductStatusDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. دریافت محصولات از دیتابیس
        var products = await _productRepository.GetListAsync(
            includeLocked: request.IncludeLocked,
            includeSold: request.IncludeSold,
            cancellationToken);

        // 2. بررسی وضعیت قفل از Redis
        var response = new List<ProductStatusDto>();
        foreach (var product in products)
        {
            var lockKey = $"product-lock:{product.Id}";
            var lockExpiry = await _productLockService
                .GetLockExpiryAsync(product.Id);

            response.Add(new ProductStatusDto(
                product.Id,
                product.Name,
                product.Price,
                Status: GetStatus(product, lockExpiry),
                IsLocked: lockExpiry != null,
                LockExpiry: lockExpiry
            ));
        }

        return Result.Ok((IReadOnlyList<ProductStatusDto>)response.AsReadOnly());
    }

    private static string GetStatus(Product product, DateTime? lockExpiry)
    {
        if (product.Status == ProductStatus.Sold) return "فروخته شده";
        return lockExpiry != null ? "قفل" : "در دسترس";
    }
}
