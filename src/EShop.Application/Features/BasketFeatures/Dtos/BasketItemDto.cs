namespace EShop.Application.Features.BasketFeatures.Dtos;
public sealed record BasketItemDto(
    Guid ProducId,
    string ProdcuName,
    int Quantity,
    decimal Price);
