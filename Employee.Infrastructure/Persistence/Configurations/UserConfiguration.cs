using Employee.Domain;
using Employee.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
         
            builder.OwnsOne(
                x => x.Username,
                username =>
                {
                    username.Property(x => x.Value)
                        .HasColumnName("Username")
                        .HasMaxLength(500)
                        .IsRequired();
                });

            builder.OwnsOne(
                x => x.PasswordHash,
                password =>
                {
                    password.Property(x => x.Value)
                        .HasColumnName("PasswordHash")
                        .HasMaxLength(500)
                        .IsRequired();
                });

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
