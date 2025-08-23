namespace HRSystem.Domain.Datasource
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Domain;
    using Microsoft.EntityFrameworkCore;

    public partial class HumanResourcesDbContext : DbContext
    {
        public HumanResourcesDbContext()
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Vacation> Vacations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
               .HasMany(e => e.SubEmployees)
               .WithOne(e => e.Manager)
               .HasForeignKey(e => e.ManagerId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Vacations)
                .WithOne(e => e.Employee)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
