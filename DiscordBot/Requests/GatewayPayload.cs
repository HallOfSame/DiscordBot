using DiscordBot.Enums;

using Newtonsoft.Json;

namespace DiscordBot.Requests
{
    public class GatewayPayload
    {
        #region Instance Properties

        [JsonProperty("d")]
        public object Data { get; set; }

        /// <summary>
        /// <c>null</c> if <see cref="OpCode" /> is not <see cref="Enums.OpCode.GatewayDispatch" />.
        /// </summary>
        [JsonProperty("t", NullValueHandling = NullValueHandling.Ignore)]
        public string EventName { get; set; }

        [JsonProperty("op")]
        public OpCode OpCode { get; set; }

        /// <summary>
        /// Used for resuming sessions & heartbeats. <c>null</c> if <see cref="OpCode" /> is not
        /// <see cref="Enums.OpCode.GatewayDispatch" />.
        /// </summary>
        [JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
        public int? SequenceNumber { get; set; }

        #endregion
    }
}