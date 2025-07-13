namespace EShop.Application.Features.BasketFeatures.Dtos;
public sealed record BasketDto(
    Guid Id,
    Guid UserId,
    IReadOnlyList<BasketItemDto> Items,
    decimal TotalPrice,
    DateTime ExpiresAt);
