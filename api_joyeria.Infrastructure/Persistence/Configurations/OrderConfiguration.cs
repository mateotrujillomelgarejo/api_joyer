using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using api_joyeria.Domain.Entities;
using api_joyeria.Domain.ValueObjects;

namespace api_joyeria.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(o => o.CreatedAt)
                .IsRequired();

            // ShippingAddress (Owned)
            builder.OwnsOne(o => o.ShippingAddress, sa =>
            {
                sa.Property(s => s.RecipientName).HasMaxLength(200);
                sa.Property(s => s.Line1).HasMaxLength(400);
                sa.Property(s => s.Line2).HasMaxLength(400);
                sa.Property(s => s.City).HasMaxLength(200);
                sa.Property(s => s.PostalCode).HasMaxLength(50);
                sa.Property(s => s.Country).HasMaxLength(100);
            });

            // Customer + Email (Owned)
            builder.OwnsOne(o => o.Customer, c =>
            {
                c.OwnsOne(cc => cc.Email, e =>
                {
                    e.Property(p => p.Value)
                     .HasColumnName("CustomerEmail")
                     .HasMaxLength(256)
                     .IsRequired();
                });

                c.Property(cc => cc.IsGuest)
                 .HasColumnName("CustomerIsGuest");
            });

            // TotalAmount (Money)
            builder.OwnsOne(o => o.TotalAmount, m =>
            {
                m.Property(p => p.Amount)
                 .HasColumnName("TotalAmount")
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();

                m.Property(p => p.Currency)
                 .HasColumnName("Currency")
                 .HasMaxLength(10)
                 .IsRequired();
            });

            // Items (colección → sin IsRequired)
            builder.HasMany(o => o.Items)
                   .WithOne()
                   .HasForeignKey("OrderId")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}