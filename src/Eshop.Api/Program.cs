using Eshop.Api;
using EShop.Application;
using EShop.Infrastructure;
using EShop.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddApiServices()
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

await builder.Services
    .MigrateDatabase(async (serviceProvider, dbContext) =>
    {
        var logger = serviceProvider
        .GetRequiredService<ILogger<AppDbContext>>();

        await AppDbContextSeeder.SeedAsync(dbContext, logger);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
