



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
using Microsoft.Extensions.Hosting;

namespace EShop.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
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


        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new ConnectionFactory
            {
                Uri = new Uri(config.GetConnectionString("RabbitMQ")!)
            };
        });
        services
            // redis lock service
            .AddScoped<IProductLockService, RedisProductLockService>()
            //rabbitmq event bus
            .AddSingleton<IEventPublisher, RabbitMQEventPublisher>()
            // payment gateway
            .AddHttpClient<IPaymentGateway, ZarinpalPaymentGateway>(client =>
            {
                client.BaseAddress = new Uri("https://api.zarinpal.com");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
        services
            .AddHostedService<PaymentBackgroundService>();

        return services;
    }
}
