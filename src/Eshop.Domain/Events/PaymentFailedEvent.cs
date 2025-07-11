using Eshop.Domain.Interfaces;

namespace Eshop.Domain.Events;
public class PaymentFailedEvent(Guid paymentId, Guid basketId) : IDomainEvent
{
    public Guid PaymentId { get; } = paymentId;
    public Guid BasketId { get; } = basketId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
