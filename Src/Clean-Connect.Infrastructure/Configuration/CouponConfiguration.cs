using Clean_Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clean_Connect.Infrastructure.Configuration
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.Code).IsUnique();

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.DiscountPercentage)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(c => c.ExpirationDate)
                .IsRequired();

            builder.Property(c => c.UsageLimit)
                .IsRequired();

            builder.Property(c => c.UsedCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
