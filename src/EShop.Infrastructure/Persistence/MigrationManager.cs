

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EShop.Infrastructure.Persistence;
public static class MigrationManager
{
    public static async Task MigrateDatabase(this IServiceCollection services, Func<IServiceProvider, AppDbContext, Task> seeder)
    {
        var serviceProvider = services.BuildServiceProvider();
        await using var scope = serviceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            //await dbContext.Database.EnsureCreatedAsync();
            logger.LogInformation("Checking for pending migrations...");

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            var pendingMigrationsList = pendingMigrations.ToList();

            if (pendingMigrationsList.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations: {Migrations}",
                    pendingMigrationsList.Count,
                    string.Join(", ", pendingMigrationsList));

                await dbContext.Database.MigrateAsync();

                logger.LogInformation("Migrations applied successfully");


            }
            else
            {
                logger.LogInformation("No pending migrations found");
            }

            await CallSeeder(seeder, serviceProvider, dbContext);
        }
        catch (SqlException e)
        {
            logger.LogError(e, "SQL error occurred while applying migrations");
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error occurred while applying migrations");
            throw;
        }
    }

    private static async Task CallSeeder(Func<IServiceProvider, AppDbContext, Task> seeder, IServiceProvider serviceProvider, AppDbContext dbContext)
    {
        await seeder(serviceProvider, dbContext);
    }
}
