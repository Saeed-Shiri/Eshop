

using EShop.Application.Contracts.Repositories;
using EShop.Application.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EShop.Application;
public static class DependencyInjections
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

    services
    // سرویس‌های پرداخت
    .AddScoped<IPaymentGateway, IPaymentGateway /*ZarinpalPaymentGateway*/>()
    .AddSingleton<IEventPublisher, IEventPublisher /*RabbitMQEventPublisher*/>()
    //.AddSingleton<IEventPublisher, RabbitMQEventPublisher>(_ =>
    //    new RabbitMQEventPublisher("amqp://localhost"))

    // ریپازیتوری‌ها
    .AddScoped<IProductRepository, IProductRepository /*SqlProductRepository*/>()
    .AddScoped<IBasketRepository, IBasketRepository /*RedisBasketRepository*/>();

        return services;
    }
}
