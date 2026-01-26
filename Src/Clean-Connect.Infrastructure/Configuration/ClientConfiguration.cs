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
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(c => c.FullName, name =>
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

            builder.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(100);
            });

            builder.OwnsOne(c => c.Address, address =>
            {
                address.Property(a => a.Value)
                    .HasColumnName("Address")
                    .IsRequired()
                    .HasMaxLength(200);
            });

            builder.OwnsOne(c => c.PhoneNumber, contact =>
            {
                contact.Property(c => c.Value)
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
                .IsRequired()
                .HasConversion<string>();
                
        }
    }
}
