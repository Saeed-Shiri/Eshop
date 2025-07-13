

using EShop.Application.Contracts.Repositories;
using EShop.Application.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EShop.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
