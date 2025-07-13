
using MediatR;

namespace EShop.Application.Features.PaymentFeatures.Commands.StartPayment;
public record StartPaymentCommand(Guid UserId) : IRequest;
