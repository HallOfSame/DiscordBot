using Newtonsoft.Json;

namespace DiscordBot.Requests.Payloads
{
    public class IdentifyPayload
    {
        #region Instance Properties

        /// <summary>
        /// Whether or not compression is supported.
        /// </summary>
        [JsonProperty("compress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Compress { get; set; }

        [JsonProperty("properties")]
        public ConnectionPropertiesClass ConnectionProperties { get; set; }

        /// <summary>
        /// Enables / Disables typing & presence events
        /// </summary>
        [JsonProperty("guild_subscriptions", NullValueHandling = NullValueHandling.Ignore)]
        public bool? GuildSubscriptions { get; set; }

        /// <summary>
        /// The <see cref="Intents" /> we want to receive.
        /// </summary>
        [JsonProperty("intents")]
        public int Intents { get; set; }

        /// <summary>
        /// Total members where offline members won't be sent.
        /// </summary>
        [JsonProperty("large_threshold", NullValueHandling = NullValueHandling.Ignore)]
        public int? LargeThreshold { get; set; }

        [JsonProperty("presence")]
        public UpdateStatusPayload Presence { get; set; }

        [JsonProperty("shard", NullValueHandling = NullValueHandling.Ignore)]
        public int[] Shard { get; set; }

        /// <summary>Authorization token.</summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        #endregion

        #region Nested type: ConnectionPropertiesClass

        public class ConnectionPropertiesClass
        {
            #region Instance Properties

            [JsonProperty("$browser")]
            public string Browser { get; set; }

            [JsonProperty("$device")]
            public string Device { get; set; }

            [JsonProperty("$os")]
            public string Os { get; set; }

            #endregion
        }

        #endregion
    }
}