using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpotifyBot.Persistence.Model.Entities;

namespace SpotifyBot.Persistence
{
    public sealed class TrackStorageService
    {
        readonly StorageUow _uow;
        public TrackStorageService(StorageUow uow) => _uow = uow;


        public async Task<Track> GetTrackById(string id)
        {
            return await _uow.Context.Tracks.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Track> GetTrackByTitle(string title)
        {
            return await _uow.Context.Tracks.FirstOrDefaultAsync(x => x.Title == title);
        }

        public async Task<Track> AddTrack(string id, string title)
        {
            var track = new Track { Id = id, Title = title };
            await _uow.Context.Tracks.AddAsync(track);
            return track;
        }

        public async Task<Track[]> GetAllTracks()
        {
            return await _uow.Context.Tracks.ToArrayAsync();
        }
    }
}
