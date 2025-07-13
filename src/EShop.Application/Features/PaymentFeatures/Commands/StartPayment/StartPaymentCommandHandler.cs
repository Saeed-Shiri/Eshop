

using Eshop.Domain.Events;
using EShop.Application.Contracts.Services;
using FluentResults;
using MediatR;

namespace EShop.Application.Features.PaymentFeatures.Commands.StartPayment;
public class StartPaymentCommandHandler : IRequestHandler<StartPaymentCommand, Result>
{
    private readonly IEventPublisher _eventPublisher;

    public StartPaymentCommandHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task<Result> Handle(StartPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _eventPublisher.EnqueuePaymentAsync(new PaymentStartedEvent(request.UserId));
            return Result.Ok();
        }
        catch (Exception ex)
        {

            return Result.Fail(ex.Message);
        }
        
        
    }
}
