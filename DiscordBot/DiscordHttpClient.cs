using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using DiscordBot.Responses;

using Newtonsoft.Json;

namespace DiscordBot
{
    public class DiscordHttpClient : IDisposable
    {
        #region Fields

        private readonly string botToken;

        private readonly HttpClient httpClient;

        #endregion

        #region Constructors

        public DiscordHttpClient(string botToken)
        {
            if (string.IsNullOrEmpty(botToken))
            {
                throw new ArgumentException("Bot Token was not set.",
                                            nameof(botToken));
            }

            this.botToken = botToken;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DiscordC#Bot", "1.0"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot",
                                                                                           botToken);
        }

        #endregion

        #region Instance Methods

        public void Dispose()
        {
            httpClient?.Dispose();
        }

        public async Task<string> GetGatewayUrlAsync(CancellationToken cancellationToken)
        {
            var uriToGet = GetUri("gateway");

            var response = await httpClient.GetAsync(uriToGet,
                                                     cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // TODO actual error handling
                throw new Exception($"Gateway response {response.StatusCode}");
            }

            var responseData = await response.Content.ReadAsStringAsync();

            var gatewayResponse = JsonConvert.DeserializeObject<GatewayResponse>(responseData);

            return gatewayResponse.Url;
        }

        private Uri GetUri(string apiEndpoint)
        {
            return new Uri($"https://discordapp.com/api/{apiEndpoint}");
        }

        #endregion
    }
}