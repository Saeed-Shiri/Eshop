
namespace EShop.Application.Features.PaymentFeatures.Dtos;
public sealed record PaymentResultDto(
    Guid PaymentId,
    string TransactionId,
    decimal Amount,
    DateTime ProcessedAt);
