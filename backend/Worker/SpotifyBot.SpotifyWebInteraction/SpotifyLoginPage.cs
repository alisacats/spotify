using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpotifyBot.SpotifyWebInteraction
{
    public static class SpotifyLoginPage
    {
        public static async Task OpenMainPage(Page page)
        {
            await page.GoToAsync("https://open.spotify.com");
        }

        // TODO: fix this selectors
        public static async Task ClickSignIn(Page page)
        {
            await page.WaitForSelectorAsync("#main > div > div.Root__top-container > div.Root__nav-bar > nav > div.NavBarFooter > div > p:nth-child(2) > button");
            await page.ClickAsync("#main > div > div.Root__top-container > div.Root__nav-bar > nav > div.NavBarFooter > div > p:nth-child(2) > button");
        }

        public static async Task<bool> SignIn(Page page, string login, string password)
        {
            var initialUrl = page.Url;
            await page.WaitForSelectorAsync("#login-username");
            await page.ClickAsync("#login-username");
            await page.Keyboard.TypeAsync(login);
            await page.ClickAsync("#login-password");
            await page.Keyboard.TypeAsync(password);
            await page.ClickAsync("#login-button");
            return initialUrl != page.Url;
        }
    }
}
