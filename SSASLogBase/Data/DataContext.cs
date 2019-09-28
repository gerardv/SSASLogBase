using SSASLogBase.Models;
using Microsoft.EntityFrameworkCore;

namespace SSASLogBase.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<SSASServer> Servers { get; set; }
        public DbSet<SSASDatabase> Databases { get; set; }
        public DbSet<Refresh> Refreshes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SourceObject> SourceObjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SSASServer>().ToTable("Server");
            modelBuilder.Entity<SSASDatabase>().ToTable("Database");
            modelBuilder.Entity<Refresh>().ToTable("Refresh");
            modelBuilder.Entity<SourceObject>().ToTable("SourceObject");

            modelBuilder.Entity<Message>()
                .ToTable("Message")
                .HasOne(m => m.Location)
                .WithOne(l => l.Message)
                .HasForeignKey<Location>(k => k.MessagId);

            modelBuilder.Entity<Location>()
                .ToTable("Location")
                .HasOne(p => p.SourceObject)
                .WithOne(i => i.Location)
                .HasForeignKey<SourceObject>(b => b.LocationId);
        }
    }
}