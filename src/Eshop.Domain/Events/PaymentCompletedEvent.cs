
using Eshop.Domain.Interfaces;

namespace Eshop.Domain.Events;
public sealed record PaymentCompletedEvent(
    Guid BasketId,
    string TransactionId,
    decimal Amount
) : DomainEventBase;