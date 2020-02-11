using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SpotifyBot.Host.Api
{
    public class ApiLauncher
    {
        public static Task RunApi(Action<IServiceCollection> servicesConfigurator) => new WebHostBuilder()
            .UseStartup<AspNetCoreStartup>()
            .ConfigureServices(servicesConfigurator)
            .ConfigureLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Error))
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseUrls("http://*:5000")
            .UseKestrel()
            .Build()
            .RunAsync();
    }
}
