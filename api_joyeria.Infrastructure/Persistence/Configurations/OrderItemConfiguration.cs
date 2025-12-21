using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                   .HasMaxLength(64)
                   .IsRequired();

            builder.Property(o => o.ProductId)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(o => o.Quantity)
                   .IsRequired();

            // Money como Value Object
            builder.OwnsOne(o => o.UnitPrice, m =>
            {
                m.Property(p => p.Amount)
                 .HasColumnName("UnitPriceAmount")
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();

                m.Property(p => p.Currency)
                 .HasColumnName("UnitPriceCurrency")
                 .HasMaxLength(10)
                 .IsRequired();
            });
        }
    }

}
