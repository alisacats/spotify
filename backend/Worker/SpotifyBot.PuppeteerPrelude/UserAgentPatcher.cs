using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpotifyBot.PuppeteerPrelude
{
    public static class UserAgentPatcher
    {
        public static async Task SetupInterceptor(Browser browser, string userAgent)
        {
            async Task InterceptPage(Page page) => await page.SetUserAgentAsync(userAgent);

            var pages = await browser.PagesAsync();
            foreach (var page in pages) await InterceptPage(page);

            browser.TargetCreated += async (obj1, args1) =>
            {
                try
                {
                    var target = args1.Target;
                    var page = await target.PageAsync();
                    if (page == null) return;
                    await InterceptPage(page);
                }
                // Protocol error (Page.createIsolatedWorld): Could not create isolated world
                catch (PuppeteerException) { }
            };
        }
    }
}
