using Microsoft.EntityFrameworkCore;
using SpotifyBot.Persistence.Model.Entities;

namespace SpotifyBot.Persistence
{
    public sealed class StorageDbContext : DbContext
    {
        public StorageDbContext(DbContextOptions opts) : base(opts) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<AccountTrackPlayStatistics>()
                .HasKey(c => new { c.AccountId, c.TrackId });
            
            modelBuilder
                .Entity<AccountTrack>()
                .HasKey(c => new { c.AccountId, c.TrackId });
        }

        public DbSet<Track> Tracks { get; set; }
        public DbSet<AccountTrack> AccountTracks { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountTrackPlayStatistics> ProfileTrackPlayStatistics { get; set; }
        public DbSet<Proxy> Proxies { get; set; }
    }
}
