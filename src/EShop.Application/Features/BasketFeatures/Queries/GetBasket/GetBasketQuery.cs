using EShop.Application.Features.BasketFeatures.Dtos;
using FluentResults;
using MediatR;

namespace EShop.Application.Features.BasketFeatures.Queries.GetBasket;
public sealed record GetBasketQuery(Guid UserId) : IRequest<Result<BasketDto>>;
