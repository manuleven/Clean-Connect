using Clean_Connect.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Infrastructure.Configuration
{
    public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
    {
        public void Configure(EntityTypeBuilder<Worker> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(w => w.FullName, name =>
            {
                name.Property(n => n.FirstName)
                .HasColumnName("FirstName")
                .IsRequired()
                .HasMaxLength(50);

                name.Property(n => n.LastName)
                .HasColumnName("LastName")
                .IsRequired()
                .HasMaxLength(50);
            });

            builder.OwnsOne(w => w.Email, email =>
            {
                email.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(100);
            });

            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(e => e.Value)
                .HasColumnName("address")
                .IsRequired()
                .HasMaxLength(200);
            });

            builder.OwnsOne(x => x.Contact, contact =>
            {
                contact.Property(e => e.Value)
                .HasColumnName("Contact")
                .IsRequired()
                .HasMaxLength(100);
            });

            builder.Property(x => x.State)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(x => x.DateOfBirth)
                .IsRequired();

            builder.Property(x => x.Gender)
                .HasConversion<string>()
                .IsRequired();

        }
    }
}
