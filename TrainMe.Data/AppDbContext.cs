using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;

namespace TrainMe.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
    }
}
