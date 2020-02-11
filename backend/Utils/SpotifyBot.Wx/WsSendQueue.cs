using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyBot.Wx
{
    public sealed class WsSendQueue
    {
        readonly WebSocket _webSocket;
        readonly TaskPool _taskQueue = new TaskPool(1);

        public WsSendQueue(WebSocket webSocket)
        {
            _webSocket = webSocket;
        }

        public async Task Send(object msg)
        {
            await _taskQueue.Put(async () =>
            {
                await _webSocket.SendJson(msg, CancellationToken.None);
                return 0;
            });
        }
    }
}
