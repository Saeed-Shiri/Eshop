

namespace EShop.Application.Features.ProductFetures.Dtos;
public sealed record ProductStatusDto(
    Guid Id,
    string Name,
    decimal Price,
    string Status, // "در دسترس", "قفل", "فروخته شده"
    bool IsLocked,
    DateTime? LockExpiry);
