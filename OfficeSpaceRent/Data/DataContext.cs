using Microsoft.EntityFrameworkCore;
using OfficeSpaceRent.Models;

namespace OfficeSpaceRent.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<OfficeSpace> OfficeSpaces { get; set; }
        public DbSet<OfficeImage> OfficeImages { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<OfficeAmenity> OfficeAmenities { get; set; }
        public DbSet<RentalRequest> RentalRequests { get; set; }
        public DbSet<Lease> Leases { get; set; }
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<OfficeAmenity>()
                .HasKey(x => new { x.OfficeSpaceId, x.AmenityId });

            modelBuilder.Entity<OfficeAmenity>()
                .HasOne(x => x.OfficeSpace)
                .WithMany(x => x.OfficeAmenities)
                .HasForeignKey(x => x.OfficeSpaceId);

            modelBuilder.Entity<OfficeAmenity>()
                .HasOne(x => x.Amenity)
                .WithMany(x => x.OfficeAmenities)
                .HasForeignKey(x => x.AmenityId);

            modelBuilder.Entity<RentalRequest>()
                .HasOne(x => x.User)
                .WithMany(x => x.RentalRequests)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RentalRequest>()
                .HasOne(x => x.OfficeSpace)
                .WithMany(x => x.RentalRequests)
                .HasForeignKey(x => x.OfficeSpaceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lease>()
                .HasOne(x => x.User)
                .WithMany(x => x.Leases)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lease>()
                .HasOne(x => x.OfficeSpace)
                .WithMany(x => x.Leases)
                .HasForeignKey(x => x.OfficeSpaceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Amenity>().HasData(
                new Amenity { Id = 1, Name = "Parking" },
                new Amenity { Id = 2, Name = "High-Speed Internet" },
                new Amenity { Id = 3, Name = "Meeting Room Access" },
                new Amenity { Id = 4, Name = "Security" },
                new Amenity { Id = 5, Name = "Reception" },
                new Amenity { Id = 6, Name = "Furnished" }
            );

            modelBuilder.Entity<OfficeSpace>().HasData(
                new OfficeSpace
                {
                    Id = 1,
                    Title = "Premium Open Space Office",
                    Description = "Modern office with city view, ideal for growing teams.",
                    Floor = 10,
                    AreaSqm = 120,
                    MonthlyRent = 4500,
                    OfficeNumber = "A-1001",
                    Address = "Axis Towers, Tbilisi",
                    IsAvailable = true,
                    IsFurnished = true,
                    Capacity = 15,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new OfficeSpace
                {
                    Id = 2,
                    Title = "Executive Office Suite",
                    Description = "Private premium suite suitable for executives or small companies.",
                    Floor = 15,
                    AreaSqm = 85,
                    MonthlyRent = 3800,
                    OfficeNumber = "B-1503",
                    Address = "Axis Towers, Tbilisi",
                    IsAvailable = true,
                    IsFurnished = true,
                    Capacity = 8,
                    CreatedAt = new DateTime(2026, 1, 1)
                }
            );

            modelBuilder.Entity<OfficeAmenity>().HasData(
                new OfficeAmenity { OfficeSpaceId = 1, AmenityId = 1 },
                new OfficeAmenity { OfficeSpaceId = 1, AmenityId = 2 },
                new OfficeAmenity { OfficeSpaceId = 1, AmenityId = 3 },
                new OfficeAmenity { OfficeSpaceId = 1, AmenityId = 4 },
                new OfficeAmenity { OfficeSpaceId = 1, AmenityId = 5 },
                new OfficeAmenity { OfficeSpaceId = 1, AmenityId = 6 },

                new OfficeAmenity { OfficeSpaceId = 2, AmenityId = 1 },
                new OfficeAmenity { OfficeSpaceId = 2, AmenityId = 2 },
                new OfficeAmenity { OfficeSpaceId = 2, AmenityId = 4 },
                new OfficeAmenity { OfficeSpaceId = 2, AmenityId = 5 },
                new OfficeAmenity { OfficeSpaceId = 2, AmenityId = 6 }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
