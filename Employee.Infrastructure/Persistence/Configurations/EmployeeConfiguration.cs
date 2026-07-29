using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Infrastructure.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Domain.Employee>
    {
        public void Configure(EntityTypeBuilder<Domain.Employee> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(e => e.Date_employee)
                .IsRequired();

            builder.OwnsOne(e => e.Name, name =>
            {
                name.Property(n => n.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.OwnsOne(e => e.Surname, surname =>
            {
                surname.Property(n => n.Value)
                    .HasColumnName("Surname")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.OwnsOne(e => e.Salary, salary =>
            {
                salary.Property(s => s.Amount)
                    .HasColumnName("Salary_amount")
                    .HasColumnType("decimal(18,2)");
            });


        }
    }
}
