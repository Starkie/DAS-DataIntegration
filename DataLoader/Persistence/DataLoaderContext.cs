namespace DataLoader.Persistence
{
    using DataLoader.Entities;
    using Microsoft.EntityFrameworkCore;

    public class DataLoaderContext : DbContext
    {

        public DataLoaderContext() { }

        public DataLoaderContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=users.db");
        }

        public DbSet<User> Users { get; set; }
    }
}