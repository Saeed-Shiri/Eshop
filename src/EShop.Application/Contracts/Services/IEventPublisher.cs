

using Eshop.Domain.Events;
using Eshop.Domain.Interfaces;

namespace EShop.Application.Contracts.Services;
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent;

    Task EnqueuePaymentAsync(PaymentStartedEvent paymentEvent);
}
