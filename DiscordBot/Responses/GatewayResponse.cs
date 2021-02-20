using Newtonsoft.Json;

namespace DiscordBot.Responses
{
    public class GatewayResponse
    {
        #region Instance Properties

        [JsonProperty("url")]
        public string Url { get; set; }

        #endregion
    }
}