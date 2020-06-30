using Microsoft.EntityFrameworkCore;
using VkBot.Data.Entities;

namespace VkBot.Data
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        public DataBaseContext(DbContextOptions options) : base(options)
        {
            if (Database.EnsureCreated())
                ConfigureDataBase();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasIndex(u => u.LastName)
                .IsUnique();
        }

        private void ConfigureDataBase()
        {
            SaveChanges();
        }
    }
}