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

        public DbSet<Artist> Artists { get; set; }

        public DbSet<UserArtistPlays> UserArtistPlays { get; set; }

        public DbSet<UserPlaysCsv> UserPlaysCsvs { get; set; }
    }
}