using System;

using DiscordBot.Enums;
using DiscordBot.Requests;
using DiscordBot.Requests.Payloads;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordBot.Tests
{
    [TestClass]
    public class PayloadDeserializeTests
    {
        [TestMethod]
        public void Can_Deserialize_Hello_Message()
        {
            var message = @"{
                              ""op"": 10,
                              ""d"": {
                                  ""heartbeat_interval"": 45000
                              }
                            }";

            var helloPayload = GetInnerPayloadAndCheckOpCode<HelloPayload>(message,
                                                                           OpCode.Hello);

            Assert.AreEqual(45000,
                            helloPayload.HeartbeatInterval);
        }

        private T GetInnerPayloadAndCheckOpCode<T>(string message,
                                                   OpCode expectedOpCode)
        {
            var payload = JsonConvert.DeserializeObject<GatewayPayload>(message);

            VerifyOpCode(payload,
                         expectedOpCode);

            var innerData = payload.Data as JToken;

            Assert.IsNotNull(innerData,
                             "Inner Data wasn't a JToken");

            return innerData.ToObject<T>();
        }

        private void VerifyOpCode(GatewayPayload payload,
                                  OpCode opCode)
        {
            Assert.AreEqual(opCode,
                            payload.OpCode,
                            "OpCode was not set correctly.");
        }

    }
}
