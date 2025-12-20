using Microsoft.EntityFrameworkCore;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    // ShippingAddress removed as DbSet because it's modeled as an owned/value object on Order

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Producto
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.SKU).HasMaxLength(100);
            entity.Property(p => p.IsDeleted).HasDefaultValue(false);

            // RowVersion / optimistic concurrency
            entity.Property(p => p.RowVersion).IsRowVersion();

            // Index on SKU for lookups
            entity.HasIndex(p => p.SKU).IsUnique(false);

            // Optional: enforce non-negative stock at DB level (SQL Server)
            // entity.HasCheckConstraint("CK_Producto_Stock_NonNegative", "[Stock] >= 0");
        });

        // Cart
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.GuestToken).IsRequired().HasMaxLength(200);
            entity.Property(c => c.CreatedAt).IsRequired();
            entity.Property(c => c.ExpiredAt);
            entity.Property(c => c.IsConsumed).HasDefaultValue(false);

            entity.HasMany(c => c.Items)
                  .WithOne(i => i.Cart)
                  .HasForeignKey(i => i.CartId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Index for fast lookup by token
            entity.HasIndex(c => c.GuestToken).HasDatabaseName("IX_Cart_GuestToken");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => ci.Id);
            entity.Property(ci => ci.ProductName).HasMaxLength(200);
            entity.Property(ci => ci.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(ci => ci.Quantity).IsRequired();
        });

        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(o => o.GuestEmail).HasMaxLength(200);
            entity.Property(o => o.ShippingCost).HasColumnType("decimal(18,2)");
            entity.Property(o => o.Tax).HasColumnType("decimal(18,2)");

            // Owned value objects for customer/shipping:
            entity.OwnsOne(o => o.Customer, nc =>
            {
                nc.Property(c => c.FullName).HasMaxLength(200).HasColumnName("Customer_FullName");
                nc.Property(c => c.Phone).HasMaxLength(50).HasColumnName("Customer_Phone");
            });

            entity.OwnsOne(o => o.Shipping, ns =>
            {
                ns.Property(s => s.Street).HasMaxLength(200).HasColumnName("Shipping_Street");
                ns.Property(s => s.City).HasMaxLength(100).HasColumnName("Shipping_City");
                ns.Property(s => s.State).HasMaxLength(100).HasColumnName("Shipping_State");
                ns.Property(s => s.ZipCode).HasMaxLength(50).HasColumnName("Shipping_ZipCode");
                ns.Property(s => s.Country).HasMaxLength(100).HasColumnName("Shipping_Country");
                ns.Property(s => s.Phone).HasMaxLength(50).HasColumnName("Shipping_Phone");
            });

            // Items and payments
            entity.HasMany(o => o.Items)
                  .WithOne()
                  .HasForeignKey(oi => oi.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(o => o.Payments)
                  .WithOne()
                  .HasForeignKey(p => p.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Index on order number for quick lookup
            entity.HasIndex(o => o.OrderNumber).IsUnique(true).HasDatabaseName("UX_Order_OrderNumber");

            // Map enum to string (optional, change to int if you prefer)
            entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(50);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.ProductName).HasMaxLength(200);
            entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(oi => oi.SKU).HasMaxLength(100);
            entity.Property(oi => oi.Quantity).IsRequired();
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            entity.Property(p => p.PaymentMethod).HasMaxLength(50);
            entity.Property(p => p.TransactionId).HasMaxLength(200);
            entity.Property(p => p.IdempotencyKey).HasMaxLength(200);
            entity.Property(p => p.GatewayResponse).HasColumnType("nvarchar(max)");

            // Map enum to string for readability (optional)
            entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);

            // Index on transaction id for quick lookup via webhook
            entity.HasIndex(p => p.TransactionId).HasDatabaseName("IX_Payment_TransactionId");
            entity.HasIndex(p => p.IdempotencyKey).HasDatabaseName("IX_Payment_IdempotencyKey");
        });
    }
}