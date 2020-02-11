using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp;
using SpotifyBot.Host.Api.Model;
using SpotifyBot.Persistence;
using SpotifyBot.Persistence.Model;
using SpotifyBot.PuppeteerPrelude;
using SpotifyBot.SpotifyWebInteraction;

namespace SpotifyBot.Host.Api
{
    public class SpotifyService
    {
        private readonly Page _page;
        private CancellationTokenSource _cancelTokenSource;
        private bool _isPlaylistPlaying;
        private int _accountId;

        private SpotifyService(Page page, int accountId)
        {
            _accountId = accountId;
            _page = page;
        }

        public static async Task<SpotifyService> Create(AccountInfo accountInfo)
        {
            var browser = await BrowserProvider.PrepareBrowser(proxy: accountInfo.Proxy == null ? null : Proxy.Parse(accountInfo.Proxy));

            var pages = await browser.PagesAsync();
            var page = pages.Single();

            await SingIn(page, accountInfo.SpotifyCredentials);

            return new SpotifyService(page, accountInfo.AccountId);
        }

        private static async Task SingIn(Page page, SpotifyCredentials spotifyCredentials)
        {
            await SpotifyLoginPage.OpenMainPage(page);
            await SpotifyLoginPage.ClickSignIn(page);
            await SpotifyLoginPage.SignIn(page, spotifyCredentials.Login, spotifyCredentials.Password);
        }

        private static PlaylistDiff GetDiff(Track[] newPlaylist, Track[] currentPlaylist)
        {
            return new PlaylistDiff()
            {
                TracksToAdd = newPlaylist.Where(x => currentPlaylist.All(y => y.Id != x.Id)).ToArray(),
                TracksToRemove = currentPlaylist.Where(x => newPlaylist.All(y => y.Id != x.Id)).ToArray()
            };
        }

        public async Task StopPlaylist()
        {
            _isPlaylistPlaying = false;

            if (_cancelTokenSource != null)
            {
                _cancelTokenSource.Cancel();
                await SpotifyControl.TogglePauseButton(_page);
            }

            _cancelTokenSource = null;
        }

        private async Task HandleTrackPlayedEvent(StorageUowProvider storageUowProvider)
        {
            var trackId = await SpotifyControl.GetTrackId(_page);

            using (var uow = storageUowProvider.CreateUow())
            {
                await uow.AccountStatisticsStorageService.HandleTrackPlayedEvent(_accountId, trackId);
                await uow.ApplyChanges();
            }
        }

        public async Task StartPlaylist(StorageUowProvider storageUowProvider)
        {
            _isPlaylistPlaying = true;
            _cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancelTokenSource.Token;

            await SpotifySidebar.OpenYourLibrary(_page);
            await SpotifyYourLibraryPage.OpenLikedSongsTab(_page);
            await SpotifyLikedSongsPage.ClickOnFirstSong(_page);
            await SpotifyControl.ActivatePlaylistRepeat(_page);
            await SpotifyControl.GoToPlayQueue(_page);

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(33000 + new Random().Next(0, 10000), token);
                await HandleTrackPlayedEvent(storageUowProvider);
                await SpotifyControl.GoToNextSong(_page);
            }
        }

        public bool IsPlaying()
        {
            var accountState = new AccountState { IsPlaying = _isPlaylistPlaying };
            return accountState.IsPlaying;
        }

        public async Task AddTrack(
            StorageUowProvider storageUowProvider,
            int accountId, string trackId, string trackTitle)
        {
            using (var uow = storageUowProvider.CreateUow())
            {
                await uow.AccountTrackService.AddTrack(accountId, trackId, trackTitle);
                await uow.ApplyChanges();
            }
        }

        public async Task RemoveTrack(StorageUowProvider storageUowProvider, string trackId)
        {
            using (var uow = storageUowProvider.CreateUow())
            {
                await uow.AccountTrackService.RemoveTrack(_accountId, trackId);
                await uow.ApplyChanges();
            }
        }

        public async Task<TrackStatistics[]> GetStatistics(StorageUowProvider storageUowProvider)
        {
            var tracksStatistic = new List<TrackStatistics>();

            using (var uow = storageUowProvider.CreateUow())
            {
                var allTracks = await uow.AccountTrackService.GetAccountTracks(_accountId);
                foreach (var track in allTracks)
                {
                    tracksStatistic.Add(new TrackStatistics()
                    {
                        Track = new Track { Id = track.Id, Title = track.Title },
                        PlaysCount = await uow.AccountStatisticsStorageService.GetTrackPlaysCount(_accountId, track.Id)
                    });
                }
                await uow.ApplyChanges();
            }

            return tracksStatistic.ToArray();
        }

        public async Task<Track[]> GetAccountPlaylist(StorageUowProvider storageUowProvider)
        {
            using (var uow = storageUowProvider.CreateUow())
            {
                var accountTracks = await uow.AccountTrackService.GetAccountTracks(_accountId);
                return accountTracks.Select(x => new Track { Id = x.Id, Title = x.Title }).ToArray();
            }
        }

        public async Task SyncPlayList(StorageUowProvider storageUowProvider, Track[] newAccountPlaylist)
        {
            var accountPlaylist = await GetAccountPlaylist(storageUowProvider);
            var playlistDiff = GetDiff(
                newAccountPlaylist,
                accountPlaylist
            );

            async Task MutateDb(Func<StorageUow, Task> dbMutator)
            {
                using (var uow = storageUowProvider.CreateUow())
                {
                    await dbMutator(uow);
                    await uow.ApplyChanges();
                }
            }

            foreach (var track in playlistDiff.TracksToAdd)
            {
                await SpotifySearchPage.Search(_page, track.Title);
                await SpotifySearchPage.ToggleSongPlaylistStatus(_page);
                await MutateDb(uow => uow.AccountTrackService.AddTrack(_accountId, track.Id, track.Title));
            }

            foreach (var track in playlistDiff.TracksToRemove)
            {
                await SpotifySearchPage.Search(_page, track.Title);
                await SpotifySearchPage.ToggleSongPlaylistStatus(_page);
                await MutateDb(uow => uow.AccountTrackService.RemoveTrack(_accountId, track.Id));
            }
        }
    }
}
