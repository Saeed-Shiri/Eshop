

using Eshop.Domain.Events;
using EShop.Application.Contracts.Repositories;
using EShop.Application.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace EShop.Infrastructure.Services.Payment;
public class PaymentBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IPaymentGateway _paymentGateway;
    private ILogger<PaymentBackgroundService> _logger;
    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;
    private const string QueueName = "payment_queue";

    public PaymentBackgroundService(
        IServiceScopeFactory scopeFactory,
        IPaymentGateway paymentGateway,
        ILogger<PaymentBackgroundService> logger,
        IConnectionFactory connectionFactory)
    {
       
        _logger = logger;
        _connectionFactory = connectionFactory;
        _paymentGateway = paymentGateway;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _connectionFactory = connectionFactory;
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

            _logger.LogInformation($"start processing basket of user with Id {message.UserId}");

            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var serviceProvider = scope.ServiceProvider;

                var basketRepository = serviceProvider
                .GetRequiredService<IBasketRepository>();

                var productRepository = serviceProvider
                .GetRequiredService<IProductRepository>();

                var lockService = serviceProvider
                .GetRequiredService<IProductLockService>();

                var basket = await basketRepository
                .GetBasketAsync(message.UserId);

                if (basket == null)
                {
                    _logger.LogWarning($"there is no basket for user with Id {message.UserId}");
                    return;
                }

                // شبیه‌سازی پرداخت
                var paymentResult = await _paymentGateway
                .ProcessPaymentAsync(
                    basket.TotalPrice,
                    "",
                    "");

                if (paymentResult.IsSuccess)
                {
                    // delete basket
                    await basketRepository
                    .DeleteBasketAsync(message.UserId);

                    
                    foreach (var item in basket.Items)
                    {
                        //mark products as sold
                        await productRepository
                        .MarkProductAsSold(item.ProductId);

                        //unlock products 
                        await lockService
                        .UnlockProductAsync(item.ProductId);
                    }


                    _logger.LogInformation($"successful payment for user with Id {message.UserId}");
                }
                else
                {
                    _logger.LogInformation($"failed payment for user with Id {message.UserId}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment failed for user {UserId}", message.UserId);
            }
        };

        await _channel!.BasicConsumeAsync(
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
