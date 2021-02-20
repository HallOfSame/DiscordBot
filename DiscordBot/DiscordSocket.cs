using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using DiscordBot.Enums;
using DiscordBot.Requests;

using Newtonsoft.Json;

namespace DiscordBot
{
    public class DiscordSocket : IDisposable
    {
        #region Fields

        private readonly BufferBlock<string> receivedMessageBlock;

        private readonly ClientWebSocket webSocket;

        #endregion

        #region Constructors

        public DiscordSocket()
        {
            webSocket = new ClientWebSocket();
            receivedMessageBlock = new BufferBlock<string>();
        }

        #endregion

        #region Instance Methods

        public async Task ConnectAsync(Uri gatewayUri,
                                       CancellationToken cancellationToken)
        {
            await webSocket.ConnectAsync(gatewayUri,
                                         cancellationToken);

#pragma warning disable 4014
            // Intentionally not awaiting here to start tasks
            CheckForNewMessages(cancellationToken);
            ProcessReceivedMessages(cancellationToken);
#pragma warning restore 4014
        }

        public void Dispose()
        {
            webSocket?.Dispose();
        }

        private async Task CheckForNewMessages(CancellationToken cancellationToken)
        {
            const int BufferSize = 4096;

            var bufferArray = new byte[BufferSize];

            var receivedMessage = string.Empty;

            while (!cancellationToken.IsCancellationRequested)
            {
                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(bufferArray,
                                                                                        0,
                                                                                        BufferSize),
                                                                 cancellationToken);

                var newBytes = new ArraySegment<byte>(bufferArray,
                                                      0,
                                                      receiveResult.Count);

                // ReSharper disable once AssignNullToNotNullAttribute don't think .Array can possibly be null here
                receivedMessage += Encoding.UTF8.GetString(newBytes.Array);

                if (!receiveResult.EndOfMessage)
                {
                    // If we have more data to get, keep receiving
                    continue;
                }

                // Add the new message to our block
                receivedMessageBlock.Post(receivedMessage);
            }
        }

        private async Task ProcessReceivedMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var newMessage = await receivedMessageBlock.ReceiveAsync();

                var decodedMessage = JsonConvert.DeserializeObject<GatewayPayload>(newMessage);

                switch (decodedMessage.OpCode)
                {
                    case OpCode.Hello:
                        // TODO load payload and start heartbeat task
                        break;
                    default:
                        Debug.WriteLine($"Unknown OpCode {decodedMessage.OpCode}");
                        break;
                }
            }
        }

        #endregion
    }
}