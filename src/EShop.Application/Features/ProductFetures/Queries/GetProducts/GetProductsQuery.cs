

using EShop.Application.Features.ProductFetures.Dtos;
using FluentResults;
using MediatR;

namespace EShop.Application.Features.ProductFetures.Queries.GetProducts;
public sealed record GetProductsQuery(
    bool IncludeLocked =false,
    bool IncludeSold = false) : IRequest<Result<IReadOnlyList<ProductStatusDto>>>;
