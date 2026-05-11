using Clean_Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clean_Connect.Infrastructure.Configuration
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.WorkerId)
                .IsRequired();

            builder.Property(x => x.Balance)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.TotalEarned)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.HasIndex(x => x.WorkerId)
                .IsUnique();

            builder.HasOne(x => x.Worker)
                .WithOne()
                .HasForeignKey<Wallet>(x => x.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore("DomainEvents");
        }
    }
}
