

using Eshop.Domain.Events;
using EShop.Application.Contracts.Repositories;
using EShop.Application.Contracts.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace EShop.Infrastructure.Services.Payment;
public class PaymentBackgroundService : BackgroundService
{
    private readonly IBasketRepository _basketRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductLockService _lockService;
    private readonly IPaymentGateway _paymentGateway;
    private ILogger<PaymentBackgroundService> _logger;
    private readonly IConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IChannel _channel;
    private const string QueueName = "payment_queue";

    public PaymentBackgroundService(
        IBasketRepository basketRepository,
        IProductRepository productRepository,
        IProductLockService lockService,
        ILogger<PaymentBackgroundService> logger,
        IConnectionFactory connectionFactory,
        IPaymentGateway paymentGateway)
    {
        _basketRepository = basketRepository;
        _productRepository = productRepository;
        _lockService = lockService;
        _logger = logger;
        _connectionFactory = connectionFactory;
        _paymentGateway = paymentGateway;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(queue: QueueName, durable: false, exclusive: false, autoDelete: false);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel!);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<PaymentStartedEvent>(json)!;

            _logger.LogInformation($"آغاز پردازش پرداخت کاربر با آبدی {message.UserId}");

            try
            {
                var basket = await _basketRepository.GetBasketAsync(message.UserId);
                if (basket == null || !basket.Items.Any())
                {
                    _logger.LogWarning($"هیچ سبد خریدی برای کابر با آیدی {message.UserId} یافت نشد");
                    return;
                }

                // شبیه‌سازی پرداخت
                var paymentResult = await _paymentGateway.ProcessPaymentAsync(
                    basket.TotalPrice,
                    "",
                    "");

                if (paymentResult.IsSuccess)
                {
                    await _basketRepository.DeleteBasketAsync(message.UserId);
                    //make products as sold
                    //unlock products 
                    _logger.LogInformation($"پرداخت برای کاربر با آیدی {message.UserId} با موفقیت انجام شد");
                }
                else
                {
                    _logger.LogInformation($"پرداخت برای کاربر با آیدی {message.UserId} با خطا مواجه شد");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment failed for user {UserId}", message.UserId);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);

    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
            await _channel.CloseAsync();

        if (_connection != null)
            await _connection.CloseAsync();

        await base.StopAsync(cancellationToken);
    }
}
