using Newtonsoft.Json;

namespace DiscordBot.Requests.Payloads
{
    public class HelloPayload
    {
        #region Instance Properties

        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }

        #endregion
    }
}