
using Eshop.Domain.Interfaces;

namespace Eshop.Domain.Events;
public sealed record PaymentCompletedEvent(
    Guid PaymentId,
    Guid BasketId,
    string TransactionId,
    decimal Amount
) : DomainEventBase;