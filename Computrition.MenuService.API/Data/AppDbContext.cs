using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.MultiTenancy;
using Computrition.MenuService.API.Utility;
using Microsoft.EntityFrameworkCore;

namespace Computrition.MenuService.API.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ITenantContext? _tenant;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenant) : base(options)
        {
            _tenant = tenant;
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        private void ApplyHospitalSlugRules()
        {
            foreach (var entry in ChangeTracker.Entries<Hospital>())
            {
                if (entry.State is not (EntityState.Added or EntityState.Modified))
                    continue;

                entry.Entity.Slug = Slug.FromName(entry.Entity.Name);
            }
        }

        public override int SaveChanges()
        {
            ApplyHospitalSlugRules();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyHospitalSlugRules();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().Property(p => p.DietaryRestrictionCode).HasConversion<string>();
                      // Hospital: unique slug
            modelBuilder.Entity<Hospital>()
                .HasIndex(h => h.Slug)
                .IsUnique();
            // FK relationships (enforced by DB)
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Hospital)
                .WithMany()
                .HasForeignKey(p => p.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Hospital)
                .WithMany()
                .HasForeignKey(m => m.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Global tenant filters (EF only)
            modelBuilder.Entity<Patient>()
                .HasQueryFilter(p => _tenant != null && p.HospitalId == _tenant.HospitalId);
            
            modelBuilder.Entity<MenuItem>()
                .HasQueryFilter(m => _tenant != null && m.HospitalId == _tenant.HospitalId);



            modelBuilder.Entity<Hospital>().HasData(
                new Hospital {Id = 1, Name = "Hospital A",Slug = "hospital-a"},
                new Hospital {Id = 2, Name = "Hospital B",Slug = "hospital-b"}
            );
            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, Name = "John Doe", DietaryRestrictionCode = DietaryRestriction.GF, HospitalId = 1 },
                new Patient { Id = 2, Name = "Jane Smith", DietaryRestrictionCode = DietaryRestriction.SF, HospitalId = 2},
                new Patient { Id = 3, Name = "Bob Brown", DietaryRestrictionCode = DietaryRestriction.None , HospitalId = 1 },
                new Patient { Id = 4, Name = "Bob Green", DietaryRestrictionCode = DietaryRestriction.HH , HospitalId = 2 }
            );

            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem { Id = 1, Name = "Gluten Free Bread", Category = "Sides", IsGlutenFree = true, HospitalId = 1 },
                new MenuItem { Id = 2, Name = "Sugar Free Jello", Category = "Dessert", IsSugarFree = true, HospitalId = 2 },
                new MenuItem { Id = 3, Name = "Regular Pasta", Category = "Main", IsGlutenFree = false, IsSugarFree = false, HospitalId = 1 },
                new MenuItem { Id = 4, Name = "Injera", Category = "Main", IsGlutenFree = false, IsSugarFree = false, HospitalId = 2 }
            );
            
            // base.OnModelCreating(modelBuilder);

        }

    }
}