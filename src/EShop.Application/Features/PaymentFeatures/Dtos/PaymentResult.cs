
namespace EShop.Application.Features.PaymentFeatures.Dtos;
public sealed record PaymentResult
{
    public bool IsSuccess { get; init; }
    public string TransactionId { get; init; }
    public decimal Amount { get; init; }
    public DateTime ProcessedAt { get; init; }
    public string Error { get; init; }

    // Factory Methods
    public static PaymentResult Success(
        string transactionId,
        decimal amount)
        => new()
        {
            IsSuccess = true,
            TransactionId = transactionId,
            Amount = amount,
            ProcessedAt = DateTime.UtcNow
        };

    public static PaymentResult Fail(
        string error)
        => new()
        {
            IsSuccess = false,
            Error = error
        };
}