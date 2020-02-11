using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpotifyBot.Wx
{
    public static class WsExtensions
    {
        // https://www.softfluent.com/blog/dev/Using-Web-Sockets-with-ASP-NET-Core

        public static ValueTask SendString(this WebSocket socket, string data, CancellationToken ct)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ReadOnlyMemory<byte>(buffer);

            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        // can return null
        public static async Task<string> ReceiveString(this WebSocket socket, CancellationToken ct = default)
        {
            var buffer = new Memory<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                ValueWebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    try
                    {
                        result = await socket.ReceiveAsync(buffer, ct);
                        if (result.MessageType == WebSocketMessageType.Close) return null;
                    }
                    catch (WebSocketException)
                    {
                        return null;
                    }

                    ms.Write(buffer.Span.Slice(0, result.Count));
                } while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                    throw new Exception("Unexpected message");

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };

        public static async Task<object> ReceiveJson(this WebSocket ws, CancellationToken ct = default)
        {
            var json = await ws.ReceiveString(ct);
            return JsonConvert.DeserializeObject(json, JsonSerializerSettings);
        }

        public static async Task SendJson(this WebSocket ws, object data, CancellationToken ct = default)
        {
            var json = JsonConvert.SerializeObject(data, JsonSerializerSettings);
            await ws.SendString(json, ct);
        }
    }
}
