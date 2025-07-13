using EShop.Application.Features.PaymentFeatures.Dtos;
using FluentResults;
using MediatR;

namespace EShop.Application.Features.PaymentFeature.Commands.ProcessPayment;
public sealed record ProcessPaymentCommand(
    Guid UserId,
    string PaymentMethod,
    string CallbackUrl) : IRequest<Result<PaymentResultDto>>;
