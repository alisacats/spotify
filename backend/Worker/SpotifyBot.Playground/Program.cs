using System;
using System.Linq;
using System.Threading.Tasks;
using SpotifyBot.PuppeteerPrelude;
using SpotifyBot.WorkerApiModel;
using SpotifyBot.WorkerServiceLayer;

namespace SpotifyBot.Playground
{
    static class Program
    {
        static readonly Proxy Proxy = Proxy.Parse("lum-customer-hl_86cfb49f-zone-test:18oo1bafp670@zproxy.lum-superproxy.io:22225");
        static readonly AccountData AccountData = new AccountData
        {
            SpotifyCredentials = new SpotifyCredentials
            {
                Login = "chillbeats3+zapej@gmail.com",
                Password = "zapej"
            }
        };

        static async Task TestPrivacy()
        {
            using (var browser = await BrowserProvider.PrepareBrowser(Proxy, x => x.Headless = true))
            {
                var pages = await browser.PagesAsync();
                var page = pages.Single();

                await page.GoToAsync("http://getright.com/useragent.html");
                var userAgent = await page.EvaluateExpressionAsync<string>("document.querySelector('center b').innerText");
                if (userAgent.Contains("Headless")) throw new Exception("WTF");
            }
        }

        static async Task Main()
        {
            // await TestPrivacy();

            Task<SpotifyService> GetService() => SpotifyService.Launch(
                accountData: AccountData,
                trackPlayedOnceHandler: _trackId => { }
            );

            using (var spotifyService = await GetService())
            {
                // play with the service here

                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}
