using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<GPS3> GPS3s { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);
                entity.Property(e => e.Id).HasColumnType("int");
            });

            modelBuilder.Entity<GPS3>(entity =>
            {
                entity.ToTable("gps3");
                entity.HasKey(u => u.Id);
            });
        }
    }
}
