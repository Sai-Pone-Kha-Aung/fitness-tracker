using Microsoft.EntityFrameworkCore;
using FitnessTracker.Models;

namespace FitnessTracker.Data
{
    /// <summary>
    /// Entity Framework database context for the Fitness Tracker application
    /// </summary>
    public class FitnessTrackerContext : DbContext
    {
        public FitnessTrackerContext(DbContextOptions<FitnessTrackerContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ActivityRecord> ActivityRecords { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(12);
                entity.Property(u => u.CalorieGoal).HasDefaultValue(0);
                entity.Property(u => u.FailedLoginAttempts).HasDefaultValue(0);
                entity.Property(u => u.IsLocked).HasDefaultValue(false);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            
            // Configure ActivityRecord entity
            modelBuilder.Entity<ActivityRecord>(entity =>
            {
                entity.HasKey(ar => ar.Id);
                entity.Property(ar => ar.ActivityType).IsRequired();
                entity.Property(ar => ar.Metric1).IsRequired().HasPrecision(10, 2);
                entity.Property(ar => ar.Metric2).IsRequired().HasPrecision(10, 2);
                entity.Property(ar => ar.Metric3).IsRequired().HasPrecision(10, 2);
                entity.Property(ar => ar.CaloriesBurned).HasPrecision(10, 2);
                entity.Property(ar => ar.RecordedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Configure relationship
                entity.HasOne(ar => ar.User)
                      .WithMany(u => u.ActivityRecords)
                      .HasForeignKey(ar => ar.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Seed some initial data for testing
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "testuser123",
                    Password = "TestPass123A", // Meets requirements: 12 chars, upper, lower
                    CalorieGoal = 300,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
