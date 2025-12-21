using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using api_joyeria.Domain.ValueObjects;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(p => p.Reference);
            builder.Property(p => p.Reference).HasMaxLength(128).IsRequired();
            builder.Property(p => p.OrderId).HasMaxLength(64).IsRequired();
            builder.Property(p => p.Status).HasConversion<string>().IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.CompletedAt).IsRequired(false);

            builder.OwnsOne(typeof(Money), "Amount", amount =>
            {
                amount.Property("Amount").HasColumnName("Amount").HasColumnType("decimal(18,2)").IsRequired();
                amount.Property("Currency").HasColumnName("Currency").HasMaxLength(10).IsRequired();
            });
        }
    }
}