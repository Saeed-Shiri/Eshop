
namespace EShop.Application.Features.PaymentFeatures.Dtos;
public sealed record PaymentResultDto(
    string TransactionId,
    decimal Amount,
    DateTime ProcessedAt);
