using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot
{
    /*
     * - open a connection
        - identify
        - start heart-beating with the interval specified in the hello payload
        - listen for events
     */

    public class Program
    {
        #region Class Methods

        public static async Task Main()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var localInfoHandler = new LocalInfoHandler();

            var localInfo = await localInfoHandler.LoadLocalInfoAsync();

            var httpClient = new DiscordHttpClient(localInfo.Token);

            var gatewayUrl = localInfo.GatewayUrl;

            if (string.IsNullOrEmpty(gatewayUrl))
            {
                gatewayUrl = await httpClient.GetGatewayUrlAsync(cancellationTokenSource.Token);

                await localInfoHandler.SaveGatewayUrlAsync(gatewayUrl);
            }

            var gatewayUri = new Uri($"{gatewayUrl}?v=8&encoding=json");

            var socket = new DiscordSocket();

            await socket.ConnectAsync(gatewayUri,
                                      cancellationTokenSource.Token);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            // TODO cleanup
            cancellationTokenSource.Cancel();

            socket.Dispose();
            httpClient.Dispose();
        }

        #endregion
    }
}