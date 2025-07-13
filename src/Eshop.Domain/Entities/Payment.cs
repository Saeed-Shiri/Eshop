
using Eshop.Domain.Common;
using Eshop.Domain.Enums;
using Eshop.Domain.Events;
using Eshop.Domain.Exceptions;

namespace Eshop.Domain.Entities;
public class Payment(
    Guid basketId,
    decimal amount) : AggregateRoot()
{
    public Guid BasketId { get; } = basketId;
    public decimal Amount { get; } = amount > 0 ? amount : throw new PaymenAmountNegativetExceptions();
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public string TransactionId { get; set; }
    public string Reason { get; set; }

    public void MarkAsCompleted(string transactionId)
    {
        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        Publish(new PaymentCompletedEvent(
            Id,
            BasketId,
            TransactionId,
            Amount));
    }

    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Faied;
        Reason = reason;
        Publish(new PaymentFailedEvent(
            Id,
            BasketId,
            Reason));
    }
}
