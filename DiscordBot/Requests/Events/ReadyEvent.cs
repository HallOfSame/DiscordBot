using Newtonsoft.Json;

namespace DiscordBot.Requests.Events
{
    /// <summary>
    /// There is more data than this, this is just all we really need to read from it.
    /// </summary>
    public class ReadyEvent
    {
        #region Instance Properties

        /// <summary>Used for resuming sessions.</summary>
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        #endregion
    }
}