using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SpotifyBot.Host.Api;
using SpotifyBot.Persistence;

namespace SpotifyBot.Host
{
    static class Program
    {
        static async Task Main()
        {
            var config = await SpotifyAccountsConfig.Read();
            var storageUowProvider = StorageUowProvider.Init();
            var spotifyServiceGroup = await SpotifyServiceGroup.Create(config, storageUowProvider);
            await ApiLauncher.RunApi(services => services
                .AddSingleton(spotifyServiceGroup)
                .AddSingleton(storageUowProvider)
            );
        }
    }
}
