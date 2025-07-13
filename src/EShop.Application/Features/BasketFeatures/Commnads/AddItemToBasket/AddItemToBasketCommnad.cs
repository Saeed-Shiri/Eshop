using EShop.Application.Features.BasketFeatures.Dtos;
using FluentResults;
using MediatR;

namespace EShop.Application.Features.basketing.Commnads.AddItemToBasket;
public sealed record AddItemToBasketCommnad(
    Guid UserId,
    Guid ProductId,
    int Quantity) : IRequest<Result<BasketDto>>;
