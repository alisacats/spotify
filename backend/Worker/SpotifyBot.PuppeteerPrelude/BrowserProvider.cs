using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpotifyBot.PuppeteerPrelude
{
    public static class BrowserProvider
    {
        const string UserAgent =
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";

        static async Task HandleProxyAuth(Browser browser, Proxy proxy)
        {
            var proxyCredentials = proxy?.Credentials;
            if (proxyCredentials == null) return;

            var puppeteerProxyCredentials = new Credentials
            {
                Username = proxyCredentials.Login,
                Password = proxyCredentials.Password
            };
            var pages = await browser.PagesAsync();
            foreach (var page in pages) await page.AuthenticateAsync(puppeteerProxyCredentials);
        }

        public static async Task<Browser> PrepareBrowser(Proxy proxy, Action<LaunchOptions> launchOptionsConfigurator = null)
        {
            var browserPath = ChromePathProvider.GetChromePath();

            var launchOptions = new LaunchOptions
            {
                ExecutablePath = browserPath,
                Headless = false,
                DefaultViewport = new ViewPortOptions { Width = 0, Height = 0 },
                Args = proxy == null ? new string[0] : new[]
                {
                    $"--proxy-server=\"{proxy.Address}\""
                }
            };
            launchOptionsConfigurator?.Invoke(launchOptions);

            var browser = await Puppeteer.LaunchAsync(launchOptions);
            await UserAgentPatcher.SetupInterceptor(browser, UserAgent);
            await HandleProxyAuth(browser, proxy);
            return browser;
        }
    }
}
