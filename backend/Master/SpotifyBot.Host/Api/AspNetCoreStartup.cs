using System;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SpotifyBot.Host.Api
{
    sealed class AspNetCoreStartup : StartupBase
    {
        static readonly TimeSpan WsKeepAlive = TimeSpan.FromSeconds(10);
        const string WsPath = "/ws";

        public override void ConfigureServices(IServiceCollection services) => services
            .With(base.ConfigureServices)
            .AddMvcCore()
            .AddFormatterMappings()
            .AddJsonFormatters();

        public override void Configure(IApplicationBuilder app) => app
            .UseMvc()
            .UseStaticFiles()
            .UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = WsKeepAlive
            })
            .Use(async (context, next) =>
            {
                if (context.Request.Path != WsPath)
                {
                    await next();
                    return;
                }

                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 400;
                    return;
                }


                var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            })
            .Run(async context =>
            {
                var env = context.RequestServices.GetService<IHostingEnvironment>();
                var fileProvider = env.WebRootFileProvider;
                var file = fileProvider.GetFileInfo("index.html");
                if (!file.Exists) return;

                using (var fileStream = file.CreateReadStream())
                {
                    await fileStream.CopyToAsync(context.Response.Body);
                }
            });
    }
}
