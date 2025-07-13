
using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using EShop.Application.Contracts.Repositories;
using EShop.Application.Contracts.Services;
using EShop.Application.Features.BasketFeatures.Dtos;
using FluentResults;
using MediatR;


namespace EShop.Application.Features.basketing.Commnads.AddItemToBasket;
public class AddItemToBasketCommnadHandler : IRequestHandler<AddItemToBasketCommnad, Result<BasketDto>>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IProductRepository _productRespository;
    private readonly IProductLockService _productLockService;

    public AddItemToBasketCommnadHandler(
        IBasketRepository basketRepository,
        IProductRepository productRespository,
        IProductLockService productLockService)
    {
        _basketRepository = basketRepository;
        _productRespository = productRespository;
        _productLockService = productLockService;
    }

    public async Task<Result<BasketDto>> Handle(AddItemToBasketCommnad request, CancellationToken cancellationToken)
    {
        // ۱. اعتبارسنجی محصول
        var product = await _productRespository
            .GetByIdAsync(request.ProductId);

        if (product?.Status != ProductStatus.Available) 
        {
            return Result.Fail("محصول ناموجود یا قفل است");
        }

        // ۲. دریافت یا ایجاد سبد خرید
        var basket = await _basketRepository
            .GetBasketAsync(request.UserId) ?? new Basket(
                Guid.NewGuid(),
                request.UserId
                );

        // ۳. قفل کردن محصول برای ۱۰ دقیقه (مطابق صورت مسئله)
        await _productLockService
            .LockProductAsync(
            request.ProductId,
            TimeSpan.FromMinutes(10));

        // ۴. افزودن آیتم به سبد
        basket.AddItem(
            new BasketItem(
                request.ProductId,
                request.Quantity,
                product.Price));

        // ۵. ذخیره تغییرات
        await _basketRepository
            .UpdateBasketAsync(basket);

        product.MarkAsLocked();
        await _productRespository
            .UpdateAsync(product);

        var response = new BasketDto
            (basket.Id,
            basket.UserId,
            basket
            .Items
            .Select(x => new BasketItemDto(
                x.ProductId,
                product.Name,
                x.Quantity,
                x.PriceAtAddition))
            .ToList()
            .AsReadOnly(),
            basket.TotalPrice,
            basket.ExpiresAt);

        return Result.Ok(response);

    }
}
