using Microsoft.EntityFrameworkCore;
using OrderApi.Models;

namespace OrderApi.Data;

public class OrderApiContext : DbContext
{
    public OrderApiContext(DbContextOptions<OrderApiContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // 1.) When Parent BE is deleted, refrence in child BE is set to null.

        modelBuilder.Entity<OrderItem>()
            .HasKey(oi => new {oi.ProductId, oi.OrderId});

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => new {oi.OrderId});

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => new {oi.ProductId});
    }
}