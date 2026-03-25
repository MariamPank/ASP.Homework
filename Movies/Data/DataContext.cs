using Microsoft.EntityFrameworkCore;
using Movies.Models;

namespace Movies.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DataContext(DbContextOptions options) : base(options)
        {
        }
    }
}
