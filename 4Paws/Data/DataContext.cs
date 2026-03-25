using _4Paws.Models;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<CareGiver> CareGivers { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Owner>()
                .HasOne(o => o.User)
                .WithOne(u => u.Owner)
                .HasForeignKey<Owner>(o => o.UserId);

            modelBuilder.Entity<CareGiver>()
                .HasOne(c => c.User)
                .WithOne(u => u.CareGiver)
                .HasForeignKey<CareGiver>(c => c.UserId);
        }

    }
}
