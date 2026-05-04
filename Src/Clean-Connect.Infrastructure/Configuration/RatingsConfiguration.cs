using Clean_Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Infrastructure.Configuration
{
    public class RatingsConfiguration : IEntityTypeConfiguration<Ratings>
    {
        public void Configure(EntityTypeBuilder<Ratings> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RatingValue)
                .IsRequired();

            builder.Property(x => x.Comment)
                .HasMaxLength(500);

            builder.HasOne(x => x.Worker)
                .WithMany(x => x.Ratings)
                .HasForeignKey(x =>  x.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Booking>()
                .WithOne(b => b.Ratings)
                .HasForeignKey<Ratings>(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasIndex(x => x.BookingId)
                .IsUnique();

            
        }
    }
}
