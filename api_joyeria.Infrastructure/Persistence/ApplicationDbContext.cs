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
    public DbSet<OrderCustomer> OrderCustomers => Set<OrderCustomer>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ShippingAddress> ShippingAddresses => Set<ShippingAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Producto
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Precio).HasColumnType("decimal(18,2)");
        });

        // Cart
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.GuestToken).IsRequired().HasMaxLength(100);
            entity.Property(c => c.CreatedAt).IsRequired();
            entity.Property(c => c.ExpiryDate).IsRequired();

            entity.HasMany(c => c.Items)
                  .WithOne()
                  .HasForeignKey(i => i.CartId);
        });

        // CartItem
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => ci.Id);
            entity.Property(ci => ci.Price).HasColumnType("decimal(18,2)");
        });

        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(o => o.GuestEmail).IsRequired().HasMaxLength(200);
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(o => o.Status).IsRequired();
            entity.Property(o => o.CreatedAt).IsRequired();

            entity.HasMany(o => o.Items)
                  .WithOne()
                  .HasForeignKey(i => i.OrderId);

            entity.HasOne(o => o.Customer)
                  .WithOne(c => c.Order)
                  .HasForeignKey<OrderCustomer>(c => c.OrderId);

            entity.HasOne(o => o.Customer)
                  .WithOne(c => c.Order)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.Price).HasColumnType("decimal(18,2)");
        });

        // OrderCustomer
        modelBuilder.Entity<OrderCustomer>(entity =>
        {
            entity.HasKey(oc => oc.Id);
            entity.Property(oc => oc.FullName).IsRequired().HasMaxLength(200);
            entity.Property(oc => oc.Phone).HasMaxLength(20);
        });

        // Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            entity.Property(p => p.PaymentMethod).HasMaxLength(50);
            entity.Property(p => p.TransactionId).HasMaxLength(100);
            entity.Property(p => p.Status).HasMaxLength(50);

            entity.HasOne(p => p.Order)
                  .WithMany()
                  .HasForeignKey(p => p.OrderId);
        });

        // ShippingAddress
        modelBuilder.Entity<ShippingAddress>(entity =>
        {
            entity.HasKey(sa => sa.Id);
            entity.Property(sa => sa.Street).HasMaxLength(200).IsRequired();
            entity.Property(sa => sa.City).HasMaxLength(100).IsRequired();
            entity.Property(sa => sa.State).HasMaxLength(100).IsRequired();
            entity.Property(sa => sa.ZipCode).HasMaxLength(20).IsRequired();
            entity.Property(sa => sa.Country).HasMaxLength(100).IsRequired();

            entity.HasOne(sa => sa.Order)
                  .WithOne()
                  .HasForeignKey<ShippingAddress>(sa => sa.OrderId);
        });
    }
}
