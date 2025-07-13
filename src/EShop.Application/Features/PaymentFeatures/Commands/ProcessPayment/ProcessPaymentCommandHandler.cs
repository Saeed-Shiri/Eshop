

using Eshop.Domain.Entities;
using Eshop.Domain.Events;
using EShop.Application.Contracts.Repositories;
using EShop.Application.Contracts.Services;
using EShop.Application.Features.PaymentFeatures.Dtos;
using FluentResults;
using MediatR;

namespace EShop.Application.Features.PaymentFeature.Commands.ProcessPayment;
public sealed class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<PaymentResultDto>>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IEventPublisher _eventPublisher;


    public ProcessPaymentCommandHandler(
        IBasketRepository basketRepository,
        IPaymentGateway paymentGateway,
        IEventPublisher eventPublisher)
    {
        _basketRepository = basketRepository;
        _paymentGateway = paymentGateway;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<PaymentResultDto>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        // 1. دریافت سبد خرید
        var basket = await _basketRepository
            .GetBasketAsync(request.UserId);

        if (basket?.Items.Count == 0)
            Result.Fail("سبد خخرید خالی است");


        // 2. پردازش پرداخت
        var paymentResult = await _paymentGateway
            .ProcessPaymentAsync(
            basket.TotalPrice,
            request.PaymentMethod,
            request.CallbackUrl);

        if (!paymentResult.IsSuccess)
        {
            // 3. انتشار ایونت شکست پرداخت
            await _eventPublisher.PublishAsync(
                new PaymentFailedEvent(
                    basket.Id,
                    paymentResult.Error));

            return Result.Fail(paymentResult.Error);
        }

        // 4. انتشار ایونت موفقیت پرداخت
        await _eventPublisher.PublishAsync(
            new PaymentCompletedEvent(
                basket.Id,
                paymentResult.TransactionId,
                basket.TotalPrice));

        return Result.Ok(new PaymentResultDto(
            paymentResult.TransactionId,
            basket.TotalPrice,
            DateTime.UtcNow));

    }
}
