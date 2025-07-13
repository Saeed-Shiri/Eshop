

using Eshop.Domain.Events;
using Eshop.Domain.Interfaces;
using EShop.Application.Contracts.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace EShop.Infrastructure.Services.EventBus;
public class RabbitMQEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private const string QueueName = "payment_queue";

    public RabbitMQEventPublisher(
        ConnectionFactory connectionFactory, 
        ILogger<RabbitMQEventPublisher> logger)
    {

        _connectionFactory = connectionFactory;
        _logger = logger;
    }



    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        try
        {
            await EnsureConnectionAsync();

            
           
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

            await _channel!.BasicPublishAsync(
                exchange: "",
                routingKey: QueueName,
                body: body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در انتشار رویداد");
            throw;
        }
    }


    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.CloseAsync();

        if (_connection != null)
            await _connection.CloseAsync();
    }

    public async Task EnqueuePaymentAsync(PaymentStartedEvent paymentEvent)
    {
        try
        {
            await EnsureConnectionAsync();

            var json = JsonSerializer.Serialize(paymentEvent);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel!.BasicPublishAsync(
                     exchange: "",
                     routingKey: QueueName,
                     body: body);

            _logger.LogInformation($"سبد خرید کاربر با آیدی {paymentEvent.UserId} به صصف پردازش ارسال شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در انتشار رویداد");
            throw;
        }
        
    }

    private async Task EnsureConnectionAsync()
    {
        if (_connection is null || !_connection.IsOpen)
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
        }

        if (_channel is null || !_channel.IsOpen)
        {
            _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false);
        }
    }
}
