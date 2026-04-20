using Microsoft.EntityFrameworkCore;
using server.Domain;

namespace server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Summary> Summaries => Set<Summary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Summary)
            .WithOne(s => s.Product)
            .HasForeignKey<Summary>(s => s.ProductId);
    }
}