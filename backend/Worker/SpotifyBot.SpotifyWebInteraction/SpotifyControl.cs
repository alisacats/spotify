using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpotifyBot.SpotifyWebInteraction
{
    public class SpotifyControl
    {
        public static async Task ActivatePlaylistRepeat(Page page)
        {
            const string script = @"(() => {
                let repeatNode = document.querySelector('.spoticon-repeat-16.control-button--active-dot');
                return repeatNode !== null;
            })()";
            var isRepeatActive = await page.EvaluateExpressionAsync<bool>(script);

            if (!isRepeatActive)
            {
                await page.ClickAsync(".spoticon-repeat-16");
            }
        }
        
        private static async Task GoToTrack(Page page, string trackId)
        {
            await page.WaitForSelectorAsync(".navBar");

            const string trackUrl = "https://open.spotify.com/track/";
            await page.GoToAsync(trackUrl + trackId);
        }

        public static async Task TogglePlayButton(Page page)
        {
            await Task.Delay(1000);

            const string script = @"(() => {
                let repeatNode = document.querySelector('button.spoticon-play-16');
                return repeatNode !== null;
            })()";
            var isPlayButtonExist = await page.EvaluateExpressionAsync<bool>(script);

            if (isPlayButtonExist)
            {
                await page.ClickAsync("button.spoticon-play-16");
            }
        }

        public static async Task TogglePauseButton(Page page)
        {
            const string selector = "button.spoticon-pause-16";
            await page.WaitForSelectorAsync(selector);
            await page.ClickAsync(selector);
        }

        public static async Task GoToNextSong(Page page)
        {
            await page.ClickAsync("button.spoticon-skip-forward-16");
        }

        public static async Task GoToPlayQueue (Page page)
        {
            await page.ClickAsync(".control-button.spoticon-queue-16");
        }

        public static async Task<string> GetTrackId(Page page)
        {
            await SpotifyContextMenu.OpenContextMenuForTheFirstSong(page);

            const string script = @"(() => document.querySelector('.react-contextmenu textarea').value)()";
            var trackLink = await page.EvaluateExpressionAsync<string>(script);

            return trackLink.Split('/').Last();
        }
    }
}
