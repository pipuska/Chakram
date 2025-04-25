using Chakram.Models;
using Microsoft.EntityFrameworkCore;

namespace Chakram.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EmployeeRate> EmployeeRates { get; set; }
        public DbSet<WorkHour> WorkHours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация для WorkHour
            modelBuilder.Entity<WorkHour>()
                .HasOne(w => w.Employee)
                .WithMany(e => e.WorkHours)
                .HasForeignKey(w => w.EmployeeId);

            // Конфигурация для EmployeeRate
            modelBuilder.Entity<EmployeeRate>()
                .HasOne(er => er.Employee)
                .WithMany(e => e.EmployeeRates)
                .HasForeignKey(er => er.EmployeeId);

            // Другие конфигурации...
        }
    }
}