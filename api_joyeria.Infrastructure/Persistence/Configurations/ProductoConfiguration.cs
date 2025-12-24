using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Infrastructure.Persistence.Configurations
{
    public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("Productos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.Nombre)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(p => p.Descripcion)
                   .HasMaxLength(500);

            builder.Property(p => p.Stock)
                   .IsRequired();

            builder.Property<string?>(nameof(Producto.ImageUrl))
                .HasMaxLength(1000)
                .IsRequired(false);

            // 🔥 VALUE OBJECT Money
            builder.OwnsOne(p => p.Price, m =>
            {
                m.Property(x => x.Amount)
                 .HasColumnName("PriceAmount")
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();

                m.Property(x => x.Currency)
                 .HasColumnName("PriceCurrency")
                 .HasMaxLength(3)
                 .IsRequired();
            });
        }
    }
}
