using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpotifyBot.SpotifyWebInteraction
{
    public static class SpotifySearchPage
    {
        public static async Task Search(Page page, string text)
        {
            await Task.Delay(2000);
            await page.ClickAsync("a[href='/search']");
            await page.Keyboard.TypeAsync(text);
        }

        // TODO: move to a separate class
        public static async Task ToggleSongPlaylistStatus(Page page)
        {
            await SpotifyContextMenu.OpenContextMenuForTheFirstSong(page);
            await page.ClickAsync(".react-contextmenu-item:nth-child(2)");
        }
    }
}
