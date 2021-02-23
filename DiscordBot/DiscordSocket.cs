using System;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using DiscordBot.Enums;
using DiscordBot.Helpers;
using DiscordBot.Requests;
using DiscordBot.Requests.Payloads;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class DiscordSocket : IDisposable
    {
        private readonly string clientToken;

        #region Fields

        private readonly BufferBlock<string> receivedMessageBlock;

        private readonly BufferBlock<string> sendMessageBlock;

        private readonly ClientWebSocket webSocket;

        private bool heartbeatAckReceived;

        private int? maxSeqNumReceived;

        #endregion

        #region Constructors

        public DiscordSocket(string clientToken)
        {
            this.clientToken = clientToken ?? throw new ArgumentNullException(nameof(clientToken));
            webSocket = new ClientWebSocket();
            receivedMessageBlock = new BufferBlock<string>();
            sendMessageBlock = new BufferBlock<string>();
        }

        #endregion

        #region Instance Methods

        public async Task ConnectAsync(Uri gatewayUri,
                                       CancellationToken cancellationToken)
        {
            await webSocket.ConnectAsync(gatewayUri, cancellationToken);

#pragma warning disable 4014
            // Intentionally not awaiting here to start tasks
            new TaskWrapper(() => CheckForNewMessages(cancellationToken)).Start();
            new TaskWrapper(() => ProcessReceivedMessages(cancellationToken)).Start();
            new TaskWrapper(() => SendMessagesTask(cancellationToken)).Start();
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
                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(bufferArray, 0, BufferSize), cancellationToken);

                var newBytes = new ArraySegment<byte>(bufferArray, 0, receiveResult.Count);

                receivedMessage += Encoding.UTF8.GetString(newBytes.ToArray());

                if (!receiveResult.EndOfMessage)
                {
                    // If we have more data to get, keep receiving
                    continue;
                }

                // Add the new message to our block
                receivedMessageBlock.Post(receivedMessage);

                receivedMessage = string.Empty;
            }
        }

        private void SendIdentify()
        {

        }

        private async Task HeartbeatTask(int interval,
                                         CancellationToken cancellationToken)
        {
            var heartbeatTimespan = TimeSpan.FromMilliseconds(interval);

            while (!cancellationToken.IsCancellationRequested)
            {
                // Send heartbeat
                var message = new GatewayPayload
                              {
                                  OpCode = OpCode.Heartbeat,
                                  Data = maxSeqNumReceived
                              };

                Debug.WriteLine("Heartbeat");

                sendMessageBlock.Post(JsonConvert.SerializeObject(message));

                // Wait interval
                await Task.Delay(heartbeatTimespan);
                
                // Check for ack
                if (heartbeatAckReceived)
                {
                    heartbeatAckReceived = false;
                    continue;
                }

                Debug.WriteLine("No ack :(");
                throw new Exception();
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
                        var helloPayload = ((JToken)decodedMessage.Data).ToObject<HelloPayload>();
#pragma warning disable 4014
                        new TaskWrapper(() => HeartbeatTask(helloPayload.HeartbeatInterval, cancellationToken)).Start();
#pragma warning restore 4014
                        break;
                    case OpCode.HeartbeatAck:
                        heartbeatAckReceived = true;
                        break;
                    default:
                        Debug.WriteLine($"Unknown OpCode {decodedMessage.OpCode}");
                        Debug.WriteLine($"Message {newMessage}.");
                        break;
                }
            }
        }

        private async Task SendMessagesTask(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var nextMessageToSend = await sendMessageBlock.ReceiveAsync();

                var messageBytes = Encoding.UTF8.GetBytes(nextMessageToSend);

                await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, cancellationToken);
            }
        }

        #endregion
    }
}