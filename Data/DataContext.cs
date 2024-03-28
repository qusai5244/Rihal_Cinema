using Microsoft.EntityFrameworkCore;
using Rihal_Cinema.Models;
using System.Diagnostics.Metrics;

namespace Rihal_Cinema.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Memory> Memories { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<MainCast> MainCasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Memory>()
            .HasOne(s => s.User)
            .WithMany(g => g.Memories)
            .HasForeignKey(s => s.UserId)
            .IsRequired();            
            
            modelBuilder.Entity<Memory>()
            .HasOne(s => s.Movie)
            .WithMany(g => g.Memories)
            .HasForeignKey(s => s.MovieId)
            .IsRequired();            
            
            modelBuilder.Entity<Rate>()
            .HasOne(s => s.User)
            .WithMany(g => g.Rates)
            .HasForeignKey(s => s.UserId)
            .IsRequired();            
            
            modelBuilder.Entity<Rate>()
            .HasOne(s => s.Movie)
            .WithMany(g => g.Rates)
            .HasForeignKey(s => s.MovieId)
            .IsRequired();            
            
            modelBuilder.Entity<Photo>()
            .HasOne(s => s.Memory)
            .WithMany(g => g.Photos)
            .HasForeignKey(s => s.MemoryId)
            .IsRequired();

            modelBuilder.Entity<MainCast>()
            .HasOne(s => s.Movie)
            .WithMany(g => g.MainCasts)
            .HasForeignKey(s => s.MovieId)
            .IsRequired();
        }
    }


}
