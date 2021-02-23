using Newtonsoft.Json;

namespace DiscordBot.Requests.Payloads
{
    public class UpdateStatusPayload
    {
        #region Instance Properties

        /// <summary>TODO</summary>
        [JsonProperty("activities")]
        public object[] Activities { get; set; }

        [JsonProperty("afk")]
        public bool Afk { get; set; }

        /// <summary>
        /// Unix time (milliseconds) when client went idle. Or null if not idle.
        /// </summary>
        [JsonProperty("since")]
        public int? Since { get; set; }

        /// <summary>
        /// The new status. <see cref="Status" />
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        #endregion
    }

    public static class Status
    {
        #region Fields

        public static readonly string Afk = "idle";

        public static readonly string DoNotDisturb = "dnd";

        public static readonly string Offline = "offline";

        public static readonly string Online = "online";

        public static readonly string ShowOffline = "invisible";

        #endregion
    }
}