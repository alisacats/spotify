using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp;
using SpotifyBot.PuppeteerPrelude;
using SpotifyBot.SpotifyWebInteraction;
using SpotifyBot.WorkerApiModel;

namespace SpotifyBot.WorkerServiceLayer
{
    public sealed class SpotifyService : IDisposable
    {
        readonly Page _page;
        CancellationTokenSource _cancelTokenSource;
        bool _isPlaylistPlaying;

        SpotifyService(Page page)
        {
            _page = page;
        }

        public void Dispose()
        {
            _page.Dispose();
            _page.Browser.Dispose();
        }

        static async Task SingIn(Page page, SpotifyCredentials spotifyCredentials)
        {
            await SpotifyLoginPage.OpenMainPage(page);
            await SpotifyLoginPage.ClickSignIn(page);
            await SpotifyLoginPage.SignIn(page, spotifyCredentials.Login, spotifyCredentials.Password);
            
            await StartPlaying(page, "3ZQgFtjJtmvUZZ8K5OBqIN");
        }
        
        private static async Task StartPlaying(Page page, string trackId)
        {

            // url = https://open.spotify.com/track/3ZQgFtjJtmvUZZ8K5OBqIN

            await page.WaitForSelectorAsync(".navBar");

            const string trackUrl = "https://open.spotify.com/track/";
            await page.GoToAsync(trackUrl + trackId);

            Random rnd = new Random();

            var needTime = 33;
            needTime *= 1000;

            while (true)
            {

                var value = rnd.Next() % 5000;
                Thread.Sleep(value + needTime);

                await SpotifyControl.GoToNextSong(page);
                await SpotifyControl.TogglePlayButton(page);
            }
        }
        
        
        
        public static async Task<SpotifyService> Launch(AccountData accountData, Action<string> trackPlayedOnceHandler)
        {
            var proxy = accountData.Proxy == null ? null : Proxy.Parse(accountData.Proxy);
            var browser = await BrowserProvider.PrepareBrowser(proxy);

            var pages = await browser.PagesAsync();
            var page = pages.Single();

            await SingIn(page, accountData.SpotifyCredentials);

            return new SpotifyService(page);
        }


        public async Task StartPlayer(string[] trackIds)
        {
            // 1. select random track
            // 2. open it
            // 3. play it [33, 40] seconds [3, 5] times
            // 4. go to 1 point
            throw new NotImplementedException();
        }

        public async Task StopPlayer()
        {
            _isPlaylistPlaying = false;

            if (_cancelTokenSource != null)
            {
                _cancelTokenSource.Cancel();
                await SpotifyControl.TogglePauseButton(_page);
            }

            _cancelTokenSource = null;
        }

        public AccountState GetState() =>
            new AccountState { IsPlaying = _isPlaylistPlaying };
    }
}
