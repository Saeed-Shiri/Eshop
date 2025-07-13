using Eshop.Domain.Interfaces;

namespace Eshop.Domain.Events;
public sealed record PaymentFailedEvent(
    Guid PaymentId,
    Guid BasketId,
    string Reason
) : DomainEventBase;
