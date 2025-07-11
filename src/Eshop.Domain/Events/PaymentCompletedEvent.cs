
using Eshop.Domain.Interfaces;

namespace Eshop.Domain.Events;
public class PaymentCompletedEvent(Guid paymentId, Guid basketId, decimal amount) : IDomainEvent
{
    public Guid PaymentId { get; } = paymentId;
    public Guid BasketId { get; } = basketId;
    public decimal Amount { get; } = amount;

    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
