using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpotifyBot.SpotifyWebInteraction
{
    public static class SpotifyYourLibraryPage
    {
        public static async Task OpenLikedSongsTab(Page page)
        {
            const string selector = "a[href='/collection/tracks']";
            await page.WaitForSelectorAsync(selector);
            await page.ClickAsync(selector);
        }
    }
}
