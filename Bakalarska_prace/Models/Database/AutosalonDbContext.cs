using Bakalarska_prace.Models.Entities;
using Bakalarska_prace.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bakalarska_prace.Models.Database
{
    public class AutosalonDbContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Cars> Cars { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<User> Users { get; set; }

        public AutosalonDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            DatabaseInit dbInit = new DatabaseInit();
            modelBuilder.Entity<Cars>().HasData(dbInit.CreateCars());

            modelBuilder.Entity<Role>().HasData(dbInit.CreateRoles());

            (User admin, List<IdentityUserRole<int>> adminRole) = dbInit.CreateAdminWithRoles();

            modelBuilder.Entity<User>().HasData(admin);
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(adminRole);
        }
    }
}
