
using Eshop.Domain.Common;
using Eshop.Domain.Enums;
using Eshop.Domain.Events;
using Eshop.Domain.Exceptions;

namespace Eshop.Domain.Entities;
public class Payment(Guid id, Guid basketId, decimal amount) : AggregateRoot(id)
{
    public Guid BasketId { get; } = basketId;
    public decimal Amount { get; } = amount > 0 ? amount : throw new PaymenAmountNegativetExceptions();
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public void MarkAsCompleted()
    {
        Status = PaymentStatus.Completed;
        Publish(new PaymentCompletedEvent(Id, BasketId, Amount));
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Faied;
        Publish(new PaymentFailedEvent(Id, BasketId));
    }
}
