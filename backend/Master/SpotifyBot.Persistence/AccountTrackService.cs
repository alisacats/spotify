using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpotifyBot.Persistence.Model.Entities;

namespace SpotifyBot.Persistence
{
    public class AccountTrackService
    {
        readonly StorageUow _uow;
        StorageDbContext Db => _uow.Context;
        public AccountTrackService(StorageUow uow) => _uow = uow;

        public Task<AccountTrack> GetAccountTrack(int accountId, string trackId) =>
            Db.AccountTracks.FirstOrDefaultAsync(x => x.AccountId == accountId && x.TrackId == trackId);

        public async Task AddTrack(int accountId, string trackId, string trackTitle)
        {
            var accountTrack = new AccountTrack
            {
                AccountId = accountId,
                TrackId = trackId
            };
            await Db.AccountTracks.AddAsync(accountTrack);

            var track = await _uow.TrackStorageService.GetTrackById(trackId);
            if (track == null) await _uow.TrackStorageService.AddTrack(trackId, trackTitle);
        }

        public async Task RemoveTrack(int accountId, string trackId)
        {
            var accountTrack = await GetAccountTrack(accountId, trackId);
            Db.AccountTracks.Remove(accountTrack);
        }

        public async Task<Track[]> GetAccountTracks(int accountId) => await Db
            .AccountTracks
            .Where(x => x.AccountId == accountId)
            .Join(Db.Tracks,
                p => p.TrackId,
                c => c.Id,
                (p, c) => new Track()
                {
                    Id = c.Id,
                    Title = c.Title
                })
            .ToArrayAsync();
    }
}
