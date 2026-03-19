using Microsoft.EntityFrameworkCore;
using SocialPosts.Models;

namespace SocialPosts.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }


        public DataContext(DbContextOptions options) : base(options)
        {
        }
    }
}
