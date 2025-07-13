using EShop.Application.Contracts.Repositories;
using EShop.Application.Features.BasketFeatures.Dtos;
using FluentResults;
using MediatR;

namespace EShop.Application.Features.BasketFeatures.Queries.GetBasket;
public sealed class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, Result<BasketDto>>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IProductRepository _productRepository;

    public GetBasketQueryHandler(
        IBasketRepository basketRepository, 
        IProductRepository productRepository)
    {
        _basketRepository = basketRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<BasketDto>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository
            .GetBasketAsync(request.UserId);

        if (basket == null)
            return Result.Fail("سبد خرید یافت نشد");

        var items = new List<BasketItemDto>();

        foreach (var item in basket.Items)
        {
            var product = await _productRepository
                .GetByIdAsync(item.ProductId);

            items.Add(new BasketItemDto(
                item.ProductId,
                product?.Name ?? "نامعلوم",
                item.Quantity,
                item.PriceAtAddition));
        }

        var response = new BasketDto(
            basket.Id,
            basket.UserId,
            items.AsReadOnly(),
            basket.TotalPrice,
            basket.ExpiresAt);

        return Result.Ok(response);
    }
}
