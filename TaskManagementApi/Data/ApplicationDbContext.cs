using Microsoft.EntityFrameworkCore;

namespace TaskManagementApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.Property(e => e.Priority)
                    .HasDefaultValue(Priority.Medium);
                    
                entity.Property(e => e.IsCompleted)
                    .HasDefaultValue(false);
                    
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                    
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}