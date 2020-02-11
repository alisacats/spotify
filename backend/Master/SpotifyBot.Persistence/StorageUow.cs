using System;
using System.Threading.Tasks;

namespace SpotifyBot.Persistence
{
    public sealed class StorageUow : IDisposable
    {
        public StorageUow(StorageDbContext ctx) => Context = ctx;
        public void Dispose() => Context.Dispose();

        public async Task ApplyChanges()
        {
            await Context.SaveChangesAsync();
        }

        public StorageDbContext Context { get; }

        public TrackStorageService TrackStorageService => new TrackStorageService(this);

        public AccountStatisticsStorageService AccountStatisticsStorageService => new AccountStatisticsStorageService(this);

        public AccountTrackService AccountTrackService => new AccountTrackService(this);
        
        public AccountStorageService AccountStorageService => new AccountStorageService(this);
        
        public ProxyStorageService ProxyStorageService => new ProxyStorageService(this);
    }
}
