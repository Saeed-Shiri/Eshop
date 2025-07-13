

using Eshop.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EShop.Infrastructure.Persistence;
public static class AppDbContextSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, ILogger<AppDbContext> logger)
    {
        if (!dbContext.Products.Any())
        {
            logger.LogInformation("پر کردن محصولات اولیه...");

            var insurances = new List<Product>
                {
                    new("محصول 1", 200) ,
                    new("محصول 2", 520) ,
                    new("محصول 3", 120) ,
                };

            await dbContext
                .Products
                .AddRangeAsync(insurances);

            await dbContext.SaveChangesAsync();
            logger.LogInformation("محصولات اولیه با موفقیت بارگذاری شدند");
        }
    }
}
