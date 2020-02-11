using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using SpotifyBot.Wx;

namespace SpotifyBot.WorkerHost
{
    class Program
    {
        static async Task TestEcho()
        {
            var ct = CancellationToken.None;

            var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri("wss://echo.websocket.org"), ct);

            const string msg = "123";
            await ws.SendString(msg, ct);
            var resp = await ws.ReceiveString(ct);
            if (resp != msg) throw new Exception("WTF");
        }

        static async Task Main()
        {
            var config = await Config.Read();
            await Task.Delay(TimeSpan.FromSeconds(config.ConnectJitterSeconds));
            await Server.Run(config);
        }
    }
}
