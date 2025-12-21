using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Infrastructure.Persistence.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
                   .HasMaxLength(64)
                   .IsRequired();

            builder.Property(ci => ci.CartId)
                   .HasMaxLength(64)
                   .IsRequired();

            builder.Property(ci => ci.ProductId)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(ci => ci.Quantity)
                   .IsRequired();
        }
    }
}
