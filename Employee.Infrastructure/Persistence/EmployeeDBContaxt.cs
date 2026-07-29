using Employee.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Employee.Infrastructure.Persistence
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
        {
        
        }
        public DbSet<Domain.Employee> Employees { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmployeeDbContext).Assembly);
        }
    }
}
