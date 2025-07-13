

using Eshop.Domain.Interfaces;

namespace Eshop.Domain.Events;
public sealed record PaymentStartedEvent(Guid UserId) : DomainEventBase;
