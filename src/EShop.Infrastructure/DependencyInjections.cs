



using EShop.Application.Contracts.Repositories;
using EShop.Application.Contracts.Services;
using EShop.Infrastructure.Persistence;
using EShop.Infrastructure.Repositories;
using EShop.Infrastructure.Services.Cache;
using EShop.Infrastructure.Services.EventBus;
using EShop.Infrastructure.Services.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace EShop.Infrastructure;
public static class DependencyInjections
{
    public static async Task<IServiceCollection> AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Ef Core
        services
            .AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("SqlServer"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
                    });
            });

        // Redis
        services
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis"); // آدرس Redis
            })
            .AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        // Repositories
        services
            .AddScoped<IBasketRepository, BasketRepository>()
            .AddScoped<IProductRepository, ProductRepository>();

        // Services
        var connectionFactory = new ConnectionFactory()
        {
            Uri = new Uri(configuration.GetConnectionString("RabbitMQ")!)
        };

        services
            // redis lock service
            .AddScoped<IProductLockService, RedisProductLockService>()
            //rabbitmq message bus
            .AddSingleton<IEventPublisher>(sp =>
                new RabbitMQEventPublisher(
                    connectionFactory,
                    sp.GetRequiredService<ILogger<RabbitMQEventPublisher>>()))
            // payment gateway
            .AddHttpClient<IPaymentGateway, ZarinpalPaymentGateway>(client =>
            {
                client.BaseAddress = new Uri("https://api.zarinpal.com");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

        return services;
    }
}
