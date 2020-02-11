using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpotifyBot.Persistence.Model.Entities;

namespace SpotifyBot.Persistence
{
    public sealed class AccountStatisticsStorageService
    {
        readonly StorageUow _uow;
        public AccountStatisticsStorageService(StorageUow uow) => _uow = uow;

        async Task<AccountTrackPlayStatistics> GetOrCreateStatistics(int accountId, string trackId)
        {
            var dbSet = _uow.Context.ProfileTrackPlayStatistics;
            var stat = await dbSet.FirstOrDefaultAsync(
                x => x.AccountId == accountId && x.TrackId == trackId
            );
            if (stat != null) return stat;

            stat = new AccountTrackPlayStatistics { AccountId = accountId, TrackId = trackId };
            await dbSet.AddAsync(stat);
            return stat;
        }

        public async Task HandleTrackPlayedEvent(int accountId, string trackId)
        {
            var stats = await GetOrCreateStatistics(accountId, trackId);
            stats.CountOfPlays++;
        }

        public async Task<int> GetTrackPlaysCount(int accountId, string trackId)
        {
            var stats = await GetOrCreateStatistics(accountId, trackId);
            return stats.CountOfPlays;
        }
    }
}
