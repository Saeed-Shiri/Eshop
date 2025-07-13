

using Eshop.Domain.Events;
using EShop.Application.Contracts.Services;
using MediatR;

namespace EShop.Application.Features.PaymentFeatures.Commands.StartPayment;
public class StartPaymentCommandHandler : IRequestHandler<StartPaymentCommand>
{
    private readonly IEventPublisher _eventPublisher;

    public StartPaymentCommandHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(StartPaymentCommand request, CancellationToken cancellationToken)
    {
        await _eventPublisher.EnqueuePaymentAsync(new PaymentStartedEvent(request.UserId));
    }
}
