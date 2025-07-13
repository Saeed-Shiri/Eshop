

using EShop.Application.Contracts.Services;
using EShop.Application.Features.PaymentFeatures.Dtos;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;

namespace EShop.Infrastructure.Services.Payment;
public class ZarinpalPaymentGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly string? _merchantId = null;

    public ZarinpalPaymentGateway(
        HttpClient httpClient,
        IConfiguration config)
    {
        _httpClient = httpClient;
        _merchantId = config["Payment:Zarinpal:MerchantId"];
    }

    
    public async Task<PaymentResult> ProcessPaymentAsync(
        decimal amount,
        string paymentMethod,
        string callbackUrl)
    {
        var request = new
        {
            MerchantID = _merchantId,
            Amount = amount,
            CallbackURL = callbackUrl
        };

        var response = await _httpClient.PostAsJsonAsync("payment/request", request);

        // پردازش پاسخ و برگرداندن نتیجه
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return PaymentResult.Fail($"خطا از زرین‌پال: {error}");
        }
        return PaymentResult.Success(
            transactionId: Guid.NewGuid().ToString(),
            request.Amount);
    }
}
