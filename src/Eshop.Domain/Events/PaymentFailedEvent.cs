using Eshop.Domain.Interfaces;

namespace Eshop.Domain.Events;
public sealed record PaymentFailedEvent(
    Guid BasketId,
    string Reason
) : DomainEventBase;
