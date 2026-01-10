using Computrition.MenuService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Computrition.MenuService.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().Property(p => p.DietaryRestrictionCode).HasConversion<string>();
            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, Name = "John Doe", DietaryRestrictionCode = DietaryRestriction.GF },
                new Patient { Id = 2, Name = "Jane Smith", DietaryRestrictionCode = DietaryRestriction.SF},
                new Patient { Id = 3, Name = "Bob Brown", DietaryRestrictionCode = DietaryRestriction.None }
            );

            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem { Id = 1, Name = "Gluten Free Bread", Category = "Sides", IsGlutenFree = true },
                new MenuItem { Id = 2, Name = "Sugar Free Jello", Category = "Dessert", IsSugarFree = true },
                new MenuItem { Id = 3, Name = "Regular Pasta", Category = "Main", IsGlutenFree = false, IsSugarFree = false }
            );
        }

    }
}