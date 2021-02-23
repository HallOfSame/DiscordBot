using System;
using System.Collections.Generic;

using DiscordBot.Constants;
using DiscordBot.Enums;
using DiscordBot.Requests;
using DiscordBot.Requests.Events;
using DiscordBot.Requests.Payloads;

using Newtonsoft.Json.Linq;

namespace DiscordBot.WebSocket
{
    public class PayloadDataExtractor
    {
        #region Fields

        private readonly Dictionary<string, Type> eventTypeMap = new Dictionary<string, Type>
                                                                 {
                                                                     {
                                                                         Events.Ready, typeof(ReadyEvent)
                                                                     },
                                                                     {
                                                                         Events.GuildCreate, typeof(GuildCreateEvent)
                                                                     }
                                                                 };

        private readonly Dictionary<OpCode, Type> opCodeTypeMap = new Dictionary<OpCode, Type>
                                                                  {
                                                                      {
                                                                          OpCode.Hello, typeof(HelloPayload)
                                                                      },
                                                                      {
                                                                          OpCode.Heartbeat, typeof(int)
                                                                      },
                                                                      {
                                                                          OpCode.HeartbeatAck, null
                                                                      },
                                                                      {
                                                                          OpCode.Identify, typeof(IdentifyPayload)
                                                                      }
                                                                  };

        #endregion

        #region Instance Methods

        public object ExtractDataFromPayload(GatewayPayload gatewayPayload)
        {
            Type dataType;

            if (gatewayPayload.OpCode == OpCode.GatewayDispatch)
            {
                dataType = eventTypeMap[gatewayPayload.EventName];
            }
            else
            {
                dataType = opCodeTypeMap[gatewayPayload.OpCode];
            }

            var deserializedData = ((JToken)gatewayPayload.Data).ToObject(dataType);

            return deserializedData;
        }

        #endregion
    }
}