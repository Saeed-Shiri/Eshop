

using Eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShop.Infrastructure.Persistence;
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // اعمال کانفیگ‌های جداول
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
