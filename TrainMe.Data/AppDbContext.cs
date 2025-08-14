using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;

namespace TrainMe.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<WorkoutItem> WorkoutItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUserEntity(modelBuilder);
            ConfigureWorkoutItemEntity(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.UserName).IsUnique();

                // One-to-many relationship with WorkoutItems
                entity.HasMany(u => u.WorkoutItems)
                    .WithOne(wi => wi.User)
                    .HasForeignKey(wi => wi.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureWorkoutItemEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkoutItem>(entity =>
            {
                // Unique constraint: User can't have duplicate workout item names on same day
                entity.HasIndex(wi => new { wi.UserId, wi.Name, wi.DayOfWeek }).IsUnique();

                // Index for sorting
                entity.HasIndex(wi => new { wi.UserId, wi.DayOfWeek, wi.SortOrder });
            });
        }
    }
}
