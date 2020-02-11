using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyBot.Host.Api.Model;
using SpotifyBot.Persistence;

namespace SpotifyBot.Host.Api
{
    [ApiController]
    [Route("/accounts/{accountId}")]
    public sealed class AccountController : ControllerBase
    {
        [Route("start-playing")]
        [HttpPost]
        public async Task<StartAccountPlayingResponse> StartPlaying(
            [FromServices] SpotifyServiceGroup spotifyServiceGroup,
            [FromServices] StorageUowProvider storageUowProvider,
            int accountId)
        {
            var spotifyService = spotifyServiceGroup.GetService(accountId);
            spotifyService.StartPlaylist(storageUowProvider);

            return new StartAccountPlayingResponse(){State = new AccountState(){IsPlaying = spotifyService.IsPlaying()} };
        }

        [Route("stop-playing")]
        [HttpPost]
        public StopAccountPlayingResponse StopPlaying([FromServices] SpotifyServiceGroup spotifyServiceGroup, int accountId)
        {
            var spotifyService = spotifyServiceGroup.GetService(accountId);
            spotifyService.StopPlaylist();

            return new StopAccountPlayingResponse(){State = new AccountState(){IsPlaying = spotifyService.IsPlaying()}};
        }

        [Route("sync-playlist")]
        [HttpPost]
        public async Task<JsonResult> SyncPlayList(
            [FromServices] SpotifyServiceGroup spotifyServiceGroup,
            [FromServices] StorageUowProvider storageUowProvider,
            int accountId,SyncAccountPlaylistRequest playList)
        {
            var spotifyService = spotifyServiceGroup.GetService(accountId);
            await spotifyService.SyncPlayList(storageUowProvider, playList.Playlist.Tracks);

            return new JsonResult(new {status = "synchronized"});
        }


        [Route("get-state")]
        public GetAccountStateResponse GetState([FromServices] SpotifyServiceGroup spotifyServiceGroup, int accountId)
        {
            var spotifyService = spotifyServiceGroup.GetService(accountId);
            return new GetAccountStateResponse(){State = new AccountState(){IsPlaying = spotifyService.IsPlaying()}};
        }

        [Route("get-playlist")]
        public async Task<GetAccountPlaylistResponse> GetAccountPlaylist(
            [FromServices] SpotifyServiceGroup spotifyServiceGroup,
            [FromServices] StorageUowProvider storageUowProvider,
            int accountId)
        {
            var spotifyService = spotifyServiceGroup.GetService(accountId);
            var accountTracks = await spotifyService.GetAccountPlaylist(storageUowProvider);
            return new GetAccountPlaylistResponse(){Playlist = new AccountPlaylist(){Tracks = accountTracks}};
        }

        [Route("add-track/{trackId}")]
        public async Task<JsonResult> AddTrack(
            [FromServices] SpotifyServiceGroup spotifyServiceGroup,
            [FromServices] StorageUowProvider storageUowProvider,
            int accountId, string trackId, string trackTitle)
        {
            var spotifyService = spotifyServiceGroup.GetService(accountId);
            await spotifyService.AddTrack(storageUowProvider, accountId, trackId, trackTitle);

            return new JsonResult(new {status = "added"});
        }

        [Route("remove-track/{trackId}")]
        public async Task<JsonResult> RemoveTrack(
            [FromServices] SpotifyServiceGroup spotifyServiceGroup,
            [FromServices] StorageUowProvider storageUowProvider,
            int accountId, string trackId)
        {
            var spotifyService = spotifyServiceGroup.GetService(accountId);
            await spotifyService.RemoveTrack(storageUowProvider, trackId);

            return new JsonResult(new {status = "removed"});
        }

        [Route("get-statistics")]
        public async Task<GetAccountStatisticsResponse> GetStatistics(
            [FromServices] SpotifyServiceGroup spotifyServiceGroup,
            [FromServices] StorageUowProvider storageUowProvider,
            int accountId)
        {
            var spotifyService = spotifyServiceGroup.GetService(accountId);
            var tracksStatistic = await spotifyService.GetStatistics(storageUowProvider);
            return new GetAccountStatisticsResponse(){Statistics = tracksStatistic};
        }
    }
}
