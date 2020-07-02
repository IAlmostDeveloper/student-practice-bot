using Microsoft.EntityFrameworkCore;
using VkBot.Data.Entities;

namespace DataBaseAccess.Data
{
    public class DataBaseContext : DbContext
    {
        public DbSet<QuestionAndAnswer> QuestionsAndAnswers { get; set; }

        public DataBaseContext(DbContextOptions options) : base(options)
        {
            if (Database.EnsureCreated())
                ConfigureDataBase();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        private void ConfigureDataBase()
        {
            SaveChanges();
        }
    }
}