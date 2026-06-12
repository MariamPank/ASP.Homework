using _4Paws.Common.Entity;
using _4Paws.Models;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.Data
{
    public class DataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<CareGiver> CareGivers { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Global Soft Delete Filter ─────────────────────────────────
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(DataContext)
                        .GetMethod(nameof(GetSoftDeleteFilter),
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                        .MakeGenericMethod(entityType.ClrType);

                    var filter = method.Invoke(null, null);
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter((System.Linq.Expressions.LambdaExpression)filter!);
                }
            }

            // =========================
            // User
            // =========================
            modelBuilder.Entity<UserModel>().HasKey(x => x.Id);
            modelBuilder.Entity<UserModel>().Property(x => x.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<UserModel>()
                .Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<UserModel>()
                .Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<UserModel>()
                .HasIndex(x => x.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            modelBuilder.Entity<UserModel>()
                .Property(x => x.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<UserModel>()
                .Property(x => x.PhoneNumber)
                .HasMaxLength(30);

            // =========================
            // Owner
            // =========================
            modelBuilder.Entity<Owner>().HasKey(x => x.Id);
            modelBuilder.Entity<Owner>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Owner>()
                .HasOne(x => x.User)
                .WithOne(x => x.Owner)
                .HasForeignKey<Owner>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Owner>()
                .HasIndex(x => x.UserId)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // =========================
            // CareGiver
            // =========================
            modelBuilder.Entity<CareGiver>().HasKey(x => x.Id);
            modelBuilder.Entity<CareGiver>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<CareGiver>()
                .HasOne(x => x.User)
                .WithOne(x => x.CareGiver)
                .HasForeignKey<CareGiver>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CareGiver>()
                .HasIndex(x => x.UserId)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            // =========================
            // Pet
            // =========================
            modelBuilder.Entity<Pet>().HasKey(x => x.Id);
            modelBuilder.Entity<Pet>().Property(x => x.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Pet>()
                .Property(x => x.PetName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Pet>()
                .Property(x => x.Description)
                .HasMaxLength(1000);

            modelBuilder.Entity<Pet>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Pets)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Listing
            // =========================
            modelBuilder.Entity<Listing>().HasKey(x => x.Id);
            modelBuilder.Entity<Listing>().Property(x => x.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Listing>()
                .Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Listing>()
                .Property(x => x.Description)
                .HasMaxLength(2000);

            modelBuilder.Entity<Listing>()
                .Property(x => x.ProposedBudget)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Listing>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Listing>()
                .HasOne(x => x.CareGiver)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.CareGiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Listing>()
                .HasOne(x => x.Pet)
                .WithMany(x => x.Listings)
                .HasForeignKey(x => x.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Application
            // =========================
            modelBuilder.Entity<Application>().HasKey(x => x.Id);
            modelBuilder.Entity<Application>().Property(x => x.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Application>()
                .Property(x => x.Message)
                .HasMaxLength(1500);

            modelBuilder.Entity<Application>()
                .Property(x => x.ProposedFee)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Application>()
                .HasOne(x => x.Listing)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Application>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Application>()
                .HasOne(x => x.CareGiver)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.CareGiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Agreement
            // =========================
            modelBuilder.Entity<Agreement>().HasKey(x => x.Id);
            modelBuilder.Entity<Agreement>().Property(x => x.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Agreement>()
                .Property(x => x.Notes)
                .HasMaxLength(2000);

            modelBuilder.Entity<Agreement>()
                .Property(x => x.AgreedFee)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Agreement>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Agreements)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agreement>()
                .HasOne(x => x.CareGiver)
                .WithMany(x => x.Agreements)
                .HasForeignKey(x => x.CareGiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agreement>()
                .HasOne(x => x.Pet)
                .WithMany(x => x.Agreements)
                .HasForeignKey(x => x.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agreement>()
                .HasOne(x => x.Listing)
                .WithMany(x => x.Agreements)
                .HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agreement>()
                .HasOne(x => x.Application)
                .WithMany(x => x.Agreements)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Review
            // =========================
            modelBuilder.Entity<Review>().HasKey(x => x.Id);
            modelBuilder.Entity<Review>().Property(x => x.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Review>()
                .Property(x => x.Comment)
                .HasMaxLength(500);

            modelBuilder.Entity<Review>()
                .HasOne(x => x.Agreement)
                .WithMany()
                .HasForeignKey(x => x.AgreementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(x => x.Reviewer)
                .WithMany()
                .HasForeignKey(x => x.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(x => x.CareGiver)
                .WithMany()
                .HasForeignKey(x => x.CareGiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(x => x.Pet)
                .WithMany()
                .HasForeignKey(x => x.PetId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static System.Linq.Expressions.Expression<Func<T, bool>> GetSoftDeleteFilter<T>()
            where T : Entity
        {
            return e => !e.IsDeleted;
        }

        public override int SaveChanges()
        {
            var modified = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified && e.Entity is Entity);
            foreach (var entry in modified)
                ((Entity)entry.Entity).UpdatedAt = DateTime.UtcNow;

            var deleted = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted && e.Entity is Entity);
            foreach (var entry in deleted)
            {
                entry.State = EntityState.Modified;
                ((Entity)entry.Entity).IsDeleted = true;
                ((Entity)entry.Entity).DeletedAt = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var modified = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified && e.Entity is Entity);
            foreach (var entry in modified)
                ((Entity)entry.Entity).UpdatedAt = DateTime.UtcNow;

            var deleted = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted && e.Entity is Entity);
            foreach (var entry in deleted)
            {
                entry.State = EntityState.Modified;
                ((Entity)entry.Entity).IsDeleted = true;
                ((Entity)entry.Entity).DeletedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(ct);
        }

    }
}
