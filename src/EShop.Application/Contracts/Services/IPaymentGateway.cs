
using EShop.Application.Features.PaymentFeatures.Dtos;

namespace EShop.Application.Contracts.Services;
public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPaymentAsync(
        decimal amount,
        string paymentMethod,
        string callbackUrl);

}
