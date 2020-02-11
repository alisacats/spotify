using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using SpotifyBot.WorkerApiModel.Events;
using SpotifyBot.WorkerApiModel.Infrastructure;
using SpotifyBot.WorkerApiModel.Requests;
using SpotifyBot.WorkerServiceLayer;
using SpotifyBot.Wx;

namespace SpotifyBot.WorkerHost
{
    public static class Server
    {
        static async Task HandleMasterMessage(ServerState state, WsSendQueue wsSendQueue, object message)
        {
            switch (message)
            {
                case SetAccountRequest setAccount:
                    state.SpotifyService?.Dispose();
                    state.SpotifyService = await SpotifyService.Launch(
                        accountData: setAccount.Account,
                        trackPlayedOnceHandler: trackId => wsSendQueue.Send(
                            new TrackPlayedOnceEvent { TrackId = trackId }
                        )
                    );
                    break;
                case StartPlayerRequest playTrack:
                    await state.SpotifyService.StartPlayer(playTrack.TrackIds);
                    break;
                case StopPlayerRequest stopPlayer:
                    await state.SpotifyService.StopPlayer();
                    break;
                default:
                    throw new Exception("WTF");
            }
        }

        static async Task ReceiveLoopIteration(ServerState state, WebSocket ws, WsSendQueue wsSendQueue)
        {
            if (!(await ws.ReceiveJson() is Request req)) throw new Exception("WTF");

            var msg = req.Body;
            try
            {
                await HandleMasterMessage(state, wsSendQueue, msg);
            }
            finally
            {
                await wsSendQueue.Send(new Response { Id = req.Id });
            }
        }

        static async Task ReceiveLoop(ServerState state, WebSocket ws, WsSendQueue wsSendQueue)
        {
            while (true)
            {
                try
                {
                    await ReceiveLoopIteration(state, ws, wsSendQueue);
                }
                catch (Exception ex)
                {
                    var msg = new ExceptionEvent { Message = ex.ToString() };
                    await wsSendQueue.Send(msg);
                }
            }
        }

        public static async Task Run(Config config)
        {
            var ws = new ClientWebSocket();
            var wsSendQueue = new WsSendQueue(ws);
            await ws.ConnectAsync(new Uri(config.MasterEndpoint), CancellationToken.None);

            var state = new ServerState();
            try
            {
                await ReceiveLoop(state, ws, wsSendQueue);
            }
            finally
            {
                state.SpotifyService?.Dispose();
            }
        }
    }
}
