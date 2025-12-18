using Microsoft.EntityFrameworkCore;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Producto
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Precio).HasColumnType("decimal(18,2)");
        });

        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.GuestName).HasMaxLength(200).IsRequired();
            entity.Property(o => o.GuestEmail).HasMaxLength(200).IsRequired();
            entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
            entity.Property(o => o.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasMany(o => o.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId);
            entity.HasOne(o => o.Customer).WithMany(c => c.Orders).HasForeignKey(o => o.CustomerId);
            entity.HasOne(o => o.ShippingAddress).WithOne(s => s.Order).HasForeignKey<ShippingAddress>(s => s.OrderId);
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(i => i.Subtotal).HasColumnType("decimal(18,2)");
        });

        // OrderCustomer
        modelBuilder.Entity<OrderCustomer>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.FullName).HasMaxLength(200).IsRequired();
            entity.Property(o => o.Email).HasMaxLength(200).IsRequired();
        });

        // Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.PaymentMethod).HasMaxLength(50);
            entity.Property(p => p.TransactionId).HasMaxLength(100);
            entity.Property(p => p.Status).HasMaxLength(50);
        });

        // Shipping Address
        modelBuilder.Entity<ShippingAddress>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Street).HasMaxLength(200).IsRequired();
            entity.Property(s => s.City).HasMaxLength(100).IsRequired();
            entity.Property(s => s.ZipCode).HasMaxLength(20).IsRequired();
        });
    }
}